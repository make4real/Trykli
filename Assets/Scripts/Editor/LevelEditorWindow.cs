using System;
using System.Collections.Generic;
using Trykli.Core;
using Trykli.Data;
using Trykli.Gameplay;
using Trykli.Localization;
using Trykli.Objectives;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>
    /// Level Editor (GDD 81): create / edit LevelData (world, number, prefab or layout, spawn, goal, inventory,
    /// objectives, time limit, difficulty, hints, reference solution), edit the layout in a scene, validate,
    /// verify the solution and play the level directly.
    /// </summary>
    public sealed class LevelEditorWindow : EditorWindow
    {
        private List<LevelData> _levels = new List<LevelData>();
        private LevelData _selected;
        private SerializedObject _serialized;
        private Vector2 _listScroll;
        private Vector2 _detailScroll;
        private string _search = "";
        private List<LevelValidator.Issue> _issues = new List<LevelValidator.Issue>();
        private string _verifyResult = "";
        private ElementType _newElementType = ElementType.Block;

        public static void Open(int levelId)
        {
            var window = GetWindow<LevelEditorWindow>("TRYKLI Level Editor");
            window.minSize = new Vector2(760f, 520f);
            window.Reload();
            if (levelId > 0) window.Select(window._levels.Find(l => l.levelId == levelId));
        }

        private void OnEnable()
        {
            Reload();
        }

        private void Reload()
        {
            _levels = LevelAssetGenerator.LoadLevelAssets();
            if (_selected != null && !_levels.Contains(_selected)) Select(null);
        }

        private void Select(LevelData level)
        {
            _selected = level;
            _serialized = level != null ? new SerializedObject(level) : null;
            _issues.Clear();
            _verifyResult = "";
            if (level != null) Selection.activeObject = level;
        }

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.BeginHorizontal();
            DrawList();
            DrawDetails();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70))) Reload();
            if (GUILayout.Button("New Level", EditorStyles.toolbarButton, GUILayout.Width(80))) CreateLevel(null);
            if (_selected != null && GUILayout.Button("Duplicate", EditorStyles.toolbarButton, GUILayout.Width(80))) CreateLevel(_selected);
            if (GUILayout.Button("Validate All", EditorStyles.toolbarButton, GUILayout.Width(90))) LevelValidationWindow.ShowWindow();
            if (GUILayout.Button("Generate from JSON", EditorStyles.toolbarButton, GUILayout.Width(130)))
            {
                TrykliMenu.GenerateLevels();
                Reload();
            }

            GUILayout.FlexibleSpace();
            _search = GUILayout.TextField(_search, EditorStyles.toolbarSearchField, GUILayout.Width(160));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawList()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(230));
            _listScroll = EditorGUILayout.BeginScrollView(_listScroll);
            if (_levels.Count == 0)
            {
                EditorGUILayout.HelpBox("No LevelData yet. Use 'Generate from JSON' or Tools > TRYKLI > Setup Project.", MessageType.Info);
            }

            int currentWorld = -1;
            foreach (LevelData level in _levels)
            {
                if (level == null) continue;
                string label = $"{level.levelId:000}  {level.Code}  {LocalizedName(level)}";
                if (!string.IsNullOrEmpty(_search) && label.IndexOf(_search, StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (level.worldId != currentWorld)
                {
                    currentWorld = level.worldId;
                    EditorGUILayout.LabelField($"World {currentWorld}", EditorStyles.boldLabel);
                }

                GUIStyle style = level == _selected ? EditorStyles.boldLabel : EditorStyles.label;
                string prefix = level == _selected ? "> " : "  ";
                if (GUILayout.Button(prefix + label + (level.isBoss ? "  [BOSS]" : ""), style)) Select(level);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawDetails()
        {
            EditorGUILayout.BeginVertical();
            if (_selected == null || _serialized == null)
            {
                EditorGUILayout.HelpBox("Select a level on the left, or create a new one.", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            _serialized.Update();
            _detailScroll = EditorGUILayout.BeginScrollView(_detailScroll);
            EditorGUILayout.LabelField($"Level {_selected.levelId:000} - {LocalizedName(_selected)}", EditorStyles.largeLabel);

            DrawActions();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
            Field("levelId");
            Field("worldId");
            Field("levelNumber");
            Field("displayNameKey");
            Field("difficulty");
            Field("isBoss");
            Field("tutorial");
            Field("simulationTimeLimit");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Content", EditorStyles.boldLabel);
            Field("levelPrefab");
            SerializedProperty layout = _serialized.FindProperty("layout");
            EditorGUILayout.PropertyField(layout.FindPropertyRelative("spawn"), new GUIContent("Spawn"));
            EditorGUILayout.PropertyField(layout.FindPropertyRelative("goal"), new GUIContent("Goal (exit)"));
            EditorGUILayout.PropertyField(layout.FindPropertyRelative("goalRadius"));
            EditorGUILayout.PropertyField(layout.FindPropertyRelative("bounds"));
            EditorGUILayout.PropertyField(layout.FindPropertyRelative("elements"), true);
            EditorGUILayout.PropertyField(layout.FindPropertyRelative("zones"), true);
            Field("availableItems", true);

            EditorGUILayout.Space();
            DrawObjectives();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Presentation & design", EditorStyles.boldLabel);
            Field("cameraSettings", true);
            Field("tipKey");
            Field("hintData", true);
            Field("solution", true);
            Field("designNotes");

            _serialized.ApplyModifiedProperties();

            if (_issues.Count > 0)
            {
                EditorGUILayout.Space();
                foreach (LevelValidator.Issue issue in _issues)
                {
                    EditorGUILayout.HelpBox(issue.Message, issue.Severity == LevelValidator.Severity.Error ? MessageType.Error : MessageType.Warning);
                }
            }

            if (!string.IsNullOrEmpty(_verifyResult)) EditorGUILayout.HelpBox(_verifyResult, MessageType.Info);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawActions()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Play level")) PlayLevel(_selected);
            if (GUILayout.Button("Validate")) ValidateSelected();
            if (GUILayout.Button("Verify solution"))
            {
                List<SolutionVerifier.Result> results = SolutionVerifierTool.Verify(new List<LevelData> { _selected }, false);
                _verifyResult = results.Count > 0 ? results[0].ToString() : "Nothing verified.";
            }

            if (GUILayout.Button("Export JSON"))
            {
                LevelAssetGenerator.ExportToJson(_selected);
                AssetDatabase.Refresh();
                _verifyResult = "Exported to " + EditorPaths.LevelJson(_selected.levelId);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            LevelAuthoringRoot root = SceneLayoutAuthoring.FindRoot();
            bool editingThis = root != null && root.level == _selected;
            if (GUILayout.Button(editingThis ? "Rebuild scene from data" : "Edit layout in scene"))
            {
                if (editingThis) SceneLayoutAuthoring.Build(_selected);
                else SceneLayoutAuthoring.Open(_selected);
            }

            GUI.enabled = editingThis;
            if (GUILayout.Button("Apply scene to LevelData"))
            {
                SceneLayoutAuthoring.Apply(root);
                _serialized = new SerializedObject(_selected);
                ValidateSelected();
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if (!editingThis) return;
            EditorGUILayout.BeginHorizontal();
            _newElementType = (ElementType)EditorGUILayout.EnumPopup(_newElementType, GUILayout.Width(160));
            if (GUILayout.Button("Add element")) SceneLayoutAuthoring.AddElement(root, _newElementType);
            if (GUILayout.Button("Add point zone")) SceneLayoutAuthoring.AddZone(root, ZoneShape.Point);
            if (GUILayout.Button("Add rect zone")) SceneLayoutAuthoring.AddZone(root, ZoneShape.Rect);
            if (GUILayout.Button("Add rail zone")) SceneLayoutAuthoring.AddZone(root, ZoneShape.Rail);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Move / rotate objects in the Scene view, edit element parameters on their ElementAuthoring " +
                                    "component, then press 'Apply scene to LevelData'. Do not save this temporary scene.", MessageType.None);
        }

        private void DrawObjectives()
        {
            EditorGUILayout.LabelField("Stars (3 objectives)", EditorStyles.boldLabel);
            SerializedProperty list = _serialized.FindProperty("starObjectives");
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                StarObjective objective = i < _selected.starObjectives.Count ? _selected.starObjectives[i] : null;
                string typeName = objective != null ? objective.GetType().Name : "(empty)";
                string description = objective != null ? objective.GetDescription() : "";
                EditorGUILayout.LabelField($"Star {i + 1}: {typeName}", EditorStyles.boldLabel);
                if (GUILayout.Button("Change type", GUILayout.Width(100))) ShowObjectiveMenu(i);
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    list.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField(description, EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(element, GUIContent.none, true);
                EditorGUILayout.EndVertical();
            }

            if (list.arraySize < StarEvaluator.StarsPerLevel && GUILayout.Button("Add objective")) ShowObjectiveMenu(-1);
        }

        private void ShowObjectiveMenu(int index)
        {
            var menu = new GenericMenu();
            foreach (string typeName in ObjectiveFactory.KnownTypes)
            {
                string captured = typeName;
                menu.AddItem(new GUIContent(typeName), false, () =>
                {
                    Undo.RecordObject(_selected, "Change objective");
                    StarObjective objective = ObjectiveFactory.CreateDefault(captured);
                    if (index < 0 || index >= _selected.starObjectives.Count) _selected.starObjectives.Add(objective);
                    else _selected.starObjectives[index] = objective;
                    EditorUtility.SetDirty(_selected);
                    _serialized = new SerializedObject(_selected);
                });
            }

            menu.ShowAsContext();
        }

        private void Field(string name, bool includeChildren = false)
        {
            SerializedProperty property = _serialized.FindProperty(name);
            if (property != null) EditorGUILayout.PropertyField(property, includeChildren);
        }

        private void ValidateSelected()
        {
            LevelDefinition definition = LevelDefinitionConverter.ToDefinition(_selected);
            _issues = LevelValidator.Validate(definition, 10, 10, ItemAssetGenerator.LoadCatalog());
            foreach (LevelData other in _levels)
            {
                if (other != _selected && other.levelId == _selected.levelId)
                    _issues.Add(new LevelValidator.Issue(_selected.levelId, LevelValidator.Severity.Error, "Duplicate levelId with " + other.name));
            }

            if (_issues.Count == 0) _issues.Add(new LevelValidator.Issue(_selected.levelId, LevelValidator.Severity.Warning, "No issue found."));
        }

        private void CreateLevel(LevelData source)
        {
            int nextId = 1;
            foreach (LevelData level in _levels) nextId = Mathf.Max(nextId, level.levelId + 1);
            string path = EditorPaths.LevelAsset(nextId);
            EditorAssetUtility.EnsureFolder(EditorPaths.LevelsFolder);
            var created = CreateInstance<LevelData>();
            if (source != null)
            {
                LevelDefinitionConverter.Apply(LevelDefinitionConverter.ToDefinition(source), created, ItemAssetGenerator.LoadCatalog());
            }
            else
            {
                created.layout = CreateDefaultLayout();
                created.starObjectives = new List<StarObjective>
                {
                    new CompleteLevelObjective(), new MaxObjectsObjective { maxObjects = 1 }, new CollectCrystalObjective { requiredCount = 1 }
                };
                created.availableItems = new List<ItemStack> { new ItemStack { item = ItemAssetGenerator.LoadCatalog().Get(BuiltInItems.Spring), count = 1 } };
            }

            created.levelId = nextId;
            created.worldId = (nextId - 1) / 10 + 1;
            created.levelNumber = (nextId - 1) % 10 + 1;
            created.displayNameKey = $"level.{nextId:000}.name";
            AssetDatabase.CreateAsset(created, path);
            AssetDatabase.SaveAssets();
            LevelAssetGenerator.RefreshDatabase();
            Reload();
            Select(created);
            Debug.Log($"[TRYKLI] Level {nextId} created at {path}. Add its name to the localization files (key {created.displayNameKey}).");
        }

        private static LevelLayout CreateDefaultLayout()
        {
            var layout = new LevelLayout { spawn = new Vector2(-3.5f, 6f), goal = new Vector2(3.5f, -2.5f) };
            ElementData ground = ElementDefaults.Create(ElementType.Block);
            ground.position = new Vector2(0f, -4.5f);
            ground.size = new Vector2(10f, 1f);
            layout.elements.Add(ground);
            ElementData crystal = ElementDefaults.Create(ElementType.Crystal);
            crystal.id = "c1";
            crystal.position = new Vector2(0f, 2f);
            layout.elements.Add(crystal);
            layout.zones.Add(new PlacementZoneData { id = "z0", shape = ZoneShape.Rect, position = new Vector2(-3f, -3.5f), size = new Vector2(3f, 0.5f) });
            return layout;
        }

        private static void PlayLevel(LevelData level)
        {
            if (EditorApplication.isPlaying) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            AssetDatabase.SaveAssets();
            SessionState.SetInt(AppInfo.EditorTestLevelKey, level.levelId);
            EditorSceneManager.OpenScene(SceneGenerator.ScenePath(SceneNames.Boot));
            EditorApplication.isPlaying = true;
        }

        private static string LocalizedName(LevelData level)
        {
            if (string.IsNullOrEmpty(level.displayNameKey)) return level.name;
            string name = Loc.Get(level.displayNameKey);
            return name == level.displayNameKey ? level.name : name;
        }
    }
}

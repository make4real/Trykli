using System.Collections.Generic;
using Trykli.Data;
using UnityEditor;
using UnityEngine;

namespace Trykli.EditorTools
{
    /// <summary>
    /// Lists validation issues of every level: missing spawn / exit, objectives, duplicates, invalid world ids,
    /// inventory, solutions and hints.
    /// </summary>
    public sealed class LevelValidationWindow : EditorWindow
    {
        private List<LevelValidator.Issue> _issues = new List<LevelValidator.Issue>();
        private Vector2 _scroll;
        private bool _showWarnings = true;
        private string _source = "";

        public static void ShowWindow()
        {
            var window = GetWindow<LevelValidationWindow>("TRYKLI Validation");
            window.Run();
        }

        public static List<LevelValidator.Issue> ValidateAssets(out int levelCount)
        {
            List<LevelData> levels = LevelAssetGenerator.LoadLevelAssets();
            var definitions = new List<LevelDefinition>();
            foreach (LevelData level in levels) definitions.Add(LevelDefinitionConverter.ToDefinition(level));
            if (definitions.Count == 0) definitions = LevelAssetGenerator.LoadDefinitions();
            levelCount = definitions.Count;
            int worldCount = Mathf.Max(1, LevelAssetGenerator.LoadWorldDefinitions().Count);
            return LevelValidator.ValidateAll(definitions, worldCount, 10, ItemAssetGenerator.LoadCatalog());
        }

        private void Run()
        {
            _issues = ValidateAssets(out int count);
            _source = $"{count} level(s) checked";
            foreach (LevelValidator.Issue issue in _issues)
            {
                if (issue.Severity == LevelValidator.Severity.Error) Debug.LogError("[TRYKLI] " + issue);
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Validate", EditorStyles.toolbarButton, GUILayout.Width(80))) Run();
            _showWarnings = GUILayout.Toggle(_showWarnings, "Warnings", EditorStyles.toolbarButton, GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            int errors = _issues.FindAll(i => i.Severity == LevelValidator.Severity.Error).Count;
            GUILayout.Label($"{_source} - {errors} error(s), {_issues.Count - errors} warning(s)");
            EditorGUILayout.EndHorizontal();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            if (_issues.Count == 0) EditorGUILayout.HelpBox("No issue found.", MessageType.Info);
            foreach (LevelValidator.Issue issue in _issues)
            {
                if (!_showWarnings && issue.Severity == LevelValidator.Severity.Warning) continue;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(issue.ToString(), issue.Severity == LevelValidator.Severity.Error ? MessageType.Error : MessageType.Warning);
                if (GUILayout.Button("Open", GUILayout.Width(60), GUILayout.Height(38))) LevelEditorWindow.Open(issue.LevelId);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}

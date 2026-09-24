#!/usr/bin/env python3
"""
Development helper used to bootstrap the TRYKLI Unity project from outside the Unity Editor.

- Generates missing .meta files under Assets/ with deterministic GUIDs (md5 of the asset path), so that
  scenes can reference scripts before Unity has ever opened the project.
- Writes the five minimal scenes (Boot, MainMenu, WorldSelect, LevelSelect, Gameplay). Each scene contains a
  single root object with its scene controller; everything else is built at runtime.
- Writes ProjectSettings/EditorBuildSettings.asset with the scenes in the right order.

Inside Unity, "Tools > TRYKLI > Setup Project" regenerates the scenes and build settings with the Unity API,
which is the reference way to repair them. This script never overwrites an existing .meta file.

Usage: python3 Tools/UnityProject/unity_assets.py
"""
import hashlib
import os
import sys

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
ASSETS = os.path.join(ROOT, "Assets")

SCENES = [
    ("Boot", "Assets/Scripts/UI/SceneControllers/BootController.cs", "BootController"),
    ("MainMenu", "Assets/Scripts/UI/SceneControllers/MainMenuController.cs", "MainMenuController"),
    ("WorldSelect", "Assets/Scripts/UI/SceneControllers/WorldSelectController.cs", "WorldSelectController"),
    ("LevelSelect", "Assets/Scripts/UI/SceneControllers/LevelSelectController.cs", "LevelSelectController"),
    ("Gameplay", "Assets/Scripts/UI/SceneControllers/GameplayController.cs", "GameplayController"),
]

IMPORTERS = {
    ".cs": "MonoImporter",
    ".asmdef": "AssemblyDefinitionImporter",
    ".json": "TextScriptImporter",
    ".txt": "TextScriptImporter",
    ".md": "TextScriptImporter",
    ".unity": "DefaultImporter",
}


def guid_for(relative_path):
    return hashlib.md5(("trykli:" + relative_path.replace(os.sep, "/")).encode("utf-8")).hexdigest()


def meta_content(relative_path, is_folder):
    guid = guid_for(relative_path)
    if is_folder:
        return ("fileFormatVersion: 2\nguid: %s\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n"
                "  userData: \n  assetBundleName: \n  assetBundleVariant: \n") % guid
    extension = os.path.splitext(relative_path)[1].lower()
    importer = IMPORTERS.get(extension, "DefaultImporter")
    if importer == "MonoImporter":
        return ("fileFormatVersion: 2\nguid: %s\nMonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n"
                "  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n  userData: \n"
                "  assetBundleName: \n  assetBundleVariant: \n") % guid
    return ("fileFormatVersion: 2\nguid: %s\n%s:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n"
            "  assetBundleVariant: \n") % (guid, importer)


def read_guid(meta_path):
    with open(meta_path) as handle:
        for line in handle:
            if line.startswith("guid:"):
                return line.split(":", 1)[1].strip()
    return None


def generate_metas():
    created = 0
    for directory, folders, files in os.walk(ASSETS):
        folders[:] = [f for f in folders if not f.startswith(".")]
        entries = [(f, True) for f in folders] + [(f, False) for f in files if not f.endswith(".meta") and not f.startswith(".")]
        for name, is_folder in entries:
            path = os.path.join(directory, name)
            meta = path + ".meta"
            if os.path.exists(meta):
                continue
            relative = os.path.relpath(path, ROOT)
            with open(meta, "w") as handle:
                handle.write(meta_content(relative, is_folder))
            created += 1
    # Remove orphan metas (asset deleted or moved).
    removed = 0
    for directory, folders, files in os.walk(ASSETS):
        for name in files:
            if name.endswith(".meta") and not os.path.exists(os.path.join(directory, name[:-5])):
                os.remove(os.path.join(directory, name))
                removed += 1
    print("metas: %d created, %d orphan removed" % (created, removed))


SCENE_TEMPLATE = """%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {fileID: 0}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 0
  m_FogColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {r: 0.212, g: 0.227, b: 0.259, a: 1}
  m_AmbientEquatorColor: {r: 0.114, g: 0.125, b: 0.133, a: 1}
  m_AmbientGroundColor: {r: 0.047, g: 0.043, b: 0.035, a: 1}
  m_AmbientIntensity: 1
  m_AmbientMode: 3
  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}
  m_SkyboxMaterial: {fileID: 0}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {fileID: 0}
  m_SpotCookie: {fileID: 10001, guid: 0000000000000000e000000000000000, type: 0}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {fileID: 0}
  m_Sun: {fileID: 0}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 0
    m_EnableRealtimeLightmaps: 0
  m_LightingDataAsset: {fileID: 0}
  m_LightingSettings: {fileID: 0}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_NavMeshData: {fileID: 0}
--- !u!1 &100000
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 100001}
  - component: {fileID: 100002}
  m_Layer: 0
  m_Name: {{NAME}}Controller
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &100001
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 100000}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {fileID: 0}
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
--- !u!114 &100002
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 100000}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: {{SCRIPT_GUID}}, type: 3}
  m_Name: 
  m_EditorClassIdentifier: 
--- !u!1660057539 &9223372036854775807
SceneRoots:
  m_ObjectHideFlags: 0
  m_Roots:
  - {fileID: 100001}
"""


def generate_scenes():
    scenes_dir = os.path.join(ASSETS, "Scenes")
    os.makedirs(scenes_dir, exist_ok=True)
    for name, script_path, _ in SCENES:
        meta = os.path.join(ROOT, script_path + ".meta")
        if not os.path.exists(meta):
            sys.exit("Missing script meta: " + meta + " (run generate_metas first)")
        path = os.path.join(scenes_dir, name + ".unity")
        with open(path, "w") as handle:
            handle.write(SCENE_TEMPLATE.replace("{{NAME}}", name).replace("{{SCRIPT_GUID}}", read_guid(meta)))
    print("scenes: %d written" % len(SCENES))


def generate_build_settings():
    lines = ["%YAML 1.1", "%TAG !u! tag:unity3d.com,2011:", "--- !u!1045 &1", "EditorBuildSettings:",
             "  m_ObjectHideFlags: 0", "  serializedVersion: 2", "  m_Scenes:"]
    for name, _, _ in SCENES:
        relative = "Assets/Scenes/%s.unity" % name
        lines += ["  - enabled: 1", "    path: " + relative, "    guid: " + guid_for(relative)]
    lines += ["  m_configObjects: {}", ""]
    with open(os.path.join(ROOT, "ProjectSettings", "EditorBuildSettings.asset"), "w") as handle:
        handle.write("\n".join(lines))
    print("build settings written")


if __name__ == "__main__":
    generate_metas()
    generate_scenes()
    generate_metas()
    generate_build_settings()

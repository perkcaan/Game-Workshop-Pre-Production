using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SceneMaker
{
    [MenuItem("Jobs/Fill New Scene")]
    private static void FillNewScene()
    {
        SceneFillDefaults defaults = AssetDatabase.LoadAssetAtPath<SceneFillDefaults>("Assets/Editor/Tools/SceneFillDefaults.asset");
        if (defaults == null)
        {
            Debug.LogError("SceneFillDefaults asset missing.");
            return;
        }

        List<GameObject> packed = defaults.packedPrefabs;
        List<GameObject> unpacked = defaults.unpackedPrefabs;
        if (packed == null) packed = new List<GameObject>();
        if (unpacked == null) unpacked = new List<GameObject>();

        // prefabs that stay packed
        foreach (GameObject prefab in packed)
        {
            if (prefab == null) continue;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(instance, "Spawn Packed Prefab");
        }

        // prefabs that get unpacked
        foreach (GameObject prefab in unpacked)
        {
            if (prefab == null) continue;

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(instance, "Spawn Unpacked Prefab");

            PrefabUtility.UnpackPrefabInstance(
                instance,
                PrefabUnpackMode.Completely,
                InteractionMode.UserAction
            );
        }

        Debug.Log("Filled scene with required prefabs");

    }
}
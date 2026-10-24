using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildSpawner
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Tools/Setup/Build Spawner")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var groundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GroundObstacle.prefab");
        var flyingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/FlyingObstacle.prefab");
        var coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Coin.prefab");

        if (groundPrefab == null || flyingPrefab == null || coinPrefab == null)
        {
            Debug.LogError("VERIFY FAILED: one or more spawn prefabs missing");
            return;
        }

        var existing = GameObject.Find("ObstacleSpawner");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        var spawner = new GameObject("ObstacleSpawner");
        var component = spawner.AddComponent<ObstacleSpawner>();

        var so = new SerializedObject(component);
        so.FindProperty("groundObstaclePrefab").objectReferenceValue = groundPrefab;
        so.FindProperty("flyingObstaclePrefab").objectReferenceValue = flyingPrefab;
        so.FindProperty("coinPrefab").objectReferenceValue = coinPrefab;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var spawner = GameObject.Find("ObstacleSpawner");
        if (spawner == null)
        {
            Debug.LogError("VERIFY FAILED: ObstacleSpawner not found after save");
            return;
        }

        var component = spawner.GetComponent<ObstacleSpawner>();
        var so = new SerializedObject(component);
        Debug.Log($"VERIFY: groundObstaclePrefab={so.FindProperty("groundObstaclePrefab").objectReferenceValue?.name}");
        Debug.Log($"VERIFY: flyingObstaclePrefab={so.FindProperty("flyingObstaclePrefab").objectReferenceValue?.name}");
        Debug.Log($"VERIFY: coinPrefab={so.FindProperty("coinPrefab").objectReferenceValue?.name}");
    }
}

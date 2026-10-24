using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildPlayerHealth
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Tools/Setup/Build Player Health")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("VERIFY FAILED: Player not found in scene");
            return;
        }

        if (player.GetComponent<PlayerHealth>() == null)
        {
            player.AddComponent<PlayerHealth>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var player = GameObject.Find("Player");
        var health = player != null ? player.GetComponent<PlayerHealth>() : null;
        Debug.Log($"VERIFY: PlayerHealth present={health != null} maxLives={(health != null ? health.MaxLives : -1)}");
    }
}

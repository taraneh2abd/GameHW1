using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildScoreTicker
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Tools/Setup/Build Score Ticker")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var existing = GameObject.Find("ScoreTicker");
        if (existing == null)
        {
            var go = new GameObject("ScoreTicker");
            go.AddComponent<ScoreTicker>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var go = GameObject.Find("ScoreTicker");
        Debug.Log($"VERIFY: ScoreTicker present={go != null && go.GetComponent<ScoreTicker>() != null}");
    }
}

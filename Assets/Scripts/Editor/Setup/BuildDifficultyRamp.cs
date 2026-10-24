using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildDifficultyRamp
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Tools/Setup/Build Difficulty Ramp")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var ticker = GameObject.Find("ScoreTicker");
        if (ticker == null)
        {
            Debug.LogError("VERIFY FAILED: ScoreTicker not found in scene");
            return;
        }

        if (ticker.GetComponent<DifficultyRamp>() == null)
        {
            ticker.AddComponent<DifficultyRamp>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var ticker = GameObject.Find("ScoreTicker");
        var ramp = ticker != null ? ticker.GetComponent<DifficultyRamp>() : null;
        Debug.Log($"VERIFY: DifficultyRamp present on ScoreTicker={ramp != null}");
    }
}

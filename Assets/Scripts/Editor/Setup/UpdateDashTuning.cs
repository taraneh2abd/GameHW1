using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// Forces the Player's dashSpeedMultiplier to the current intended value,
/// overriding any stale value already baked into the scene.
public static class UpdateDashTuning
{
    private const string ScenePath = "Assets/Scenes/Game.unity";
    private const float DashSpeedMultiplier = 1.5f;

    [MenuItem("Tools/Setup/Update Dash Tuning")]
    public static void Update()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("VERIFY FAILED: Player not found");
            return;
        }

        var controller = player.GetComponent<PlayerController>();
        var so = new SerializedObject(controller);
        so.FindProperty("dashSpeedMultiplier").floatValue = DashSpeedMultiplier;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var player = GameObject.Find("Player");
        var controller = player.GetComponent<PlayerController>();
        var so = new SerializedObject(controller);
        Debug.Log($"VERIFY: dashSpeedMultiplier={so.FindProperty("dashSpeedMultiplier").floatValue} playerPosition={player.transform.position}");
    }
}

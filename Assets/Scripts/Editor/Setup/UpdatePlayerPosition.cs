using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// Moves the Player to the left side of the camera view (standard runner
/// framing) instead of dead-center, without touching anything else wired on
/// the GameObject.
public static class UpdatePlayerPosition
{
    private const string ScenePath = "Assets/Scenes/Game.unity";
    private const float PlayerX = -2.5f;

    [MenuItem("Tools/Setup/Update Player Position")]
    public static void Update()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("VERIFY FAILED: Player not found");
            return;
        }

        player.transform.position = new Vector3(PlayerX, player.transform.position.y, player.transform.position.z);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var player = GameObject.Find("Player");
        Debug.Log($"VERIFY: Player pos={player.transform.position}");
    }
}

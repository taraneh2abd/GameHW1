using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class BuildMainMenu
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Tools/Setup/Build Main Menu")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("VERIFY FAILED: Canvas not found, build the HUD first");
            return;
        }

        var existing = canvas.transform.Find("MainMenuPanel");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        var panel = MenuUiHelpers.CreateFullScreenPanel("MainMenuPanel", canvas.transform);

        MenuUiHelpers.CreateText(panel.transform, "Title", "DINO RUNNER", 48,
            new Vector2(0.5f, 0.65f), new Vector2(500f, 80f));

        MenuUiHelpers.CreateButton(panel.transform, "PlayButton", "Play",
            new Vector2(0.5f, 0.45f), new Vector2(220f, 60f));

        MenuUiHelpers.CreateButton(panel.transform, "QuitButton", "Quit",
            new Vector2(0.5f, 0.3f), new Vector2(220f, 60f));

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var panel = GameObject.Find("MainMenuPanel");
        if (panel == null)
        {
            Debug.LogError("VERIFY FAILED: MainMenuPanel missing after save");
            return;
        }

        Debug.Log($"VERIFY: MainMenuPanel active={panel.activeSelf} childCount={panel.transform.childCount}");
        foreach (Transform child in panel.transform)
        {
            Debug.Log($"VERIFY: child={child.name}");
        }
    }
}

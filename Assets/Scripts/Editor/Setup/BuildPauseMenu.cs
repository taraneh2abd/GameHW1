using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildPauseMenu
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Tools/Setup/Build Pause Menu")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("VERIFY FAILED: Canvas not found, build the HUD first");
            return;
        }

        var existing = canvas.transform.Find("PausePanel");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        var panel = MenuUiHelpers.CreateFullScreenPanel("PausePanel", canvas.transform);
        panel.SetActive(false);

        MenuUiHelpers.CreateText(panel.transform, "Title", "PAUSED", 40,
            new Vector2(0.5f, 0.7f), new Vector2(400f, 70f));

        MenuUiHelpers.CreateButton(panel.transform, "ResumeButton", "Resume",
            new Vector2(0.5f, 0.52f), new Vector2(220f, 55f));

        MenuUiHelpers.CreateButton(panel.transform, "RestartButton", "Restart",
            new Vector2(0.5f, 0.4f), new Vector2(220f, 55f));

        MenuUiHelpers.CreateButton(panel.transform, "QuitButton", "Main Menu",
            new Vector2(0.5f, 0.28f), new Vector2(220f, 55f));

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var canvas = GameObject.Find("Canvas");
        var panelTransform = canvas != null ? canvas.transform.Find("PausePanel") : null;
        var panel = panelTransform != null ? panelTransform.gameObject : null;
        if (panel == null)
        {
            Debug.LogError("VERIFY FAILED: PausePanel missing after save");
            return;
        }

        Debug.Log($"VERIFY: PausePanel active={panel.activeSelf} childCount={panel.transform.childCount}");
        foreach (Transform child in panel.transform)
        {
            Debug.Log($"VERIFY: child={child.name}");
        }
    }
}

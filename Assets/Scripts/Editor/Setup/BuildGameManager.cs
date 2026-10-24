using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public static class BuildGameManager
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Tools/Setup/Build Game Manager")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var canvas = GameObject.Find("Canvas");
        var mainMenuPanel = canvas.transform.Find("MainMenuPanel").gameObject;
        var pausePanel = canvas.transform.Find("PausePanel").gameObject;
        var hud = canvas.transform.Find("Hud").gameObject;
        var player = GameObject.Find("Player");
        var obstacleSpawner = GameObject.Find("ObstacleSpawner");
        var scoreTicker = GameObject.Find("ScoreTicker");
        var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Input/GameControls.inputactions");

        var gameOverPanel = BuildGameOverPanel(canvas.transform, out var finalScoreText);

        var existing = GameObject.Find("GameManager");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        var managerObj = new GameObject("GameManager");
        var manager = managerObj.AddComponent<GameManager>();

        var so = new SerializedObject(manager);
        so.FindProperty("inputActions").objectReferenceValue = inputActions;
        so.FindProperty("mainMenuPanel").objectReferenceValue = mainMenuPanel;
        so.FindProperty("pausePanel").objectReferenceValue = pausePanel;
        so.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
        so.FindProperty("finalScoreText").objectReferenceValue = finalScoreText;
        so.FindProperty("hud").objectReferenceValue = hud;
        so.FindProperty("player").objectReferenceValue = player;
        so.FindProperty("obstacleSpawner").objectReferenceValue = obstacleSpawner;
        so.FindProperty("scoreTicker").objectReferenceValue = scoreTicker;
        so.ApplyModifiedPropertiesWithoutUndo();

        BindButton(mainMenuPanel, "PlayButton", manager, manager.StartGame);
        BindButton(mainMenuPanel, "QuitButton", manager, manager.QuitApplication);

        BindButton(pausePanel, "ResumeButton", manager, manager.ResumeGame);
        BindButton(pausePanel, "RestartButton", manager, manager.RetryGame);
        BindButton(pausePanel, "QuitButton", manager, manager.QuitToMainMenu);

        BindButton(gameOverPanel, "RetryButton", manager, manager.RetryGame);
        BindButton(gameOverPanel, "MainMenuButton", manager, manager.QuitToMainMenu);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static GameObject BuildGameOverPanel(Transform canvasTransform, out Text finalScoreText)
    {
        var existing = canvasTransform.Find("GameOverPanel");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        var panel = MenuUiHelpers.CreateFullScreenPanel("GameOverPanel", canvasTransform);
        panel.SetActive(false);

        MenuUiHelpers.CreateText(panel.transform, "Title", "GAME OVER", 44,
            new Vector2(0.5f, 0.68f), new Vector2(450f, 70f));

        finalScoreText = MenuUiHelpers.CreateText(panel.transform, "FinalScoreText", "Score: 0", 26,
            new Vector2(0.5f, 0.56f), new Vector2(300f, 40f));

        MenuUiHelpers.CreateButton(panel.transform, "RetryButton", "Retry",
            new Vector2(0.5f, 0.42f), new Vector2(220f, 55f));

        MenuUiHelpers.CreateButton(panel.transform, "MainMenuButton", "Main Menu",
            new Vector2(0.5f, 0.3f), new Vector2(220f, 55f));

        return panel;
    }

    private static void BindButton(GameObject panel, string buttonName, GameManager target, UnityEngine.Events.UnityAction call)
    {
        var buttonTransform = panel.transform.Find(buttonName);
        if (buttonTransform == null)
        {
            Debug.LogError($"VERIFY FAILED: button '{buttonName}' not found under {panel.name}");
            return;
        }

        var button = buttonTransform.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(button.onClick, call);
    }

    private static void Verify()
    {
        var manager = Object.FindFirstObjectByType<GameManager>();
        if (manager == null)
        {
            Debug.LogError("VERIFY FAILED: GameManager missing after save");
            return;
        }

        var so = new SerializedObject(manager);
        string[] fields =
        {
            "inputActions", "mainMenuPanel", "pausePanel", "gameOverPanel",
            "finalScoreText", "hud", "player", "obstacleSpawner", "scoreTicker",
        };
        foreach (var field in fields)
        {
            Debug.Log($"VERIFY: {field}={so.FindProperty(field).objectReferenceValue?.name}");
        }

        var canvas = GameObject.Find("Canvas");
        string[] panels = { "MainMenuPanel", "PausePanel", "GameOverPanel" };
        foreach (var panelName in panels)
        {
            var panel = canvas.transform.Find(panelName).gameObject;
            foreach (var btn in panel.GetComponentsInChildren<Button>(true))
            {
                Debug.Log($"VERIFY: {panelName}/{btn.name} persistentEventCount={btn.onClick.GetPersistentEventCount()}");
            }
        }
    }
}

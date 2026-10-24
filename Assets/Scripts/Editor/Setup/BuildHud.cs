using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class BuildHud
{
    private const string ScenePath = "Assets/Scenes/Game.unity";

    [MenuItem("Tools/Setup/Build Hud")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        EnsureEventSystem();
        var canvas = EnsureCanvas();
        BuildHudPanel(canvas.transform);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<InputSystemUIInputModule>();
    }

    private static Canvas EnsureCanvas()
    {
        var existing = GameObject.Find("Canvas");
        if (existing != null)
        {
            return existing.GetComponent<Canvas>();
        }

        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960, 540);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    private static void BuildHudPanel(Transform canvasTransform)
    {
        var existing = canvasTransform.Find("Hud");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }

        var hud = new GameObject("Hud", typeof(RectTransform));
        hud.transform.SetParent(canvasTransform, false);
        StretchFull((RectTransform)hud.transform);

        var healthBg = CreateImage("HealthBarBackground", hud.transform, new Color(0.15f, 0.15f, 0.15f, 0.8f));
        SetRect(healthBg.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -20f), new Vector2(200f, 24f));

        var healthFill = CreateImage("HealthBarFill", hud.transform, new Color(0.25f, 0.8f, 0.3f));
        healthFill.type = Image.Type.Filled;
        healthFill.fillMethod = Image.FillMethod.Horizontal;
        healthFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthFill.fillAmount = 1f;
        SetRect(healthFill.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -20f), new Vector2(200f, 24f));

        var currentScoreText = CreateText("CurrentScoreText", hud.transform, "Score: 0", TextAnchor.UpperRight);
        SetRect(currentScoreText.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20f, -20f), new Vector2(220f, 30f));

        var highScoreText = CreateText("HighScoreText", hud.transform, "Best: 0", TextAnchor.UpperRight);
        SetRect(highScoreText.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20f, -54f), new Vector2(220f, 30f));

        var controller = hud.AddComponent<HudController>();
        var so = new SerializedObject(controller);
        so.FindProperty("healthFill").objectReferenceValue = healthFill;
        so.FindProperty("currentScoreText").objectReferenceValue = currentScoreText;
        so.FindProperty("highScoreText").objectReferenceValue = highScoreText;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void StretchFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = anchorMin;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
    }

    private static Image CreateImage(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var image = go.AddComponent<Image>();
        image.color = color;
        return image;
    }

    private static Text CreateText(string name, Transform parent, string initialText, TextAnchor alignment)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = initialText;
        text.alignment = alignment;
        text.color = Color.white;
        text.fontSize = 22;
        return text;
    }

    private static void Verify()
    {
        var hud = GameObject.Find("Hud");
        if (hud == null)
        {
            Debug.LogError("VERIFY FAILED: Hud not found after save");
            return;
        }

        var controller = hud.GetComponent<HudController>();
        var so = new SerializedObject(controller);
        Debug.Log($"VERIFY: healthFill={so.FindProperty("healthFill").objectReferenceValue?.name}");
        Debug.Log($"VERIFY: currentScoreText={so.FindProperty("currentScoreText").objectReferenceValue?.name}");
        Debug.Log($"VERIFY: highScoreText={so.FindProperty("highScoreText").objectReferenceValue?.name}");
        Debug.Log($"VERIFY: EventSystem present={Object.FindFirstObjectByType<EventSystem>() != null}");
        Debug.Log($"VERIFY: InputSystemUIInputModule present={Object.FindFirstObjectByType<InputSystemUIInputModule>() != null}");
    }
}

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// Replaces the HUD's fill-bar health display with three heart icons.
public static class UpdateHudHearts
{
    private const string ScenePath = "Assets/Scenes/Game.unity";
    private const string HeartSheetPath = "Assets/Sprites/UI/HeartIcons.png";
    private const int HeartCount = 3;
    private static readonly Vector2 HeartSize = new Vector2(36f, 36f);
    private const float HeartSpacing = 40f;

    [MenuItem("Tools/Setup/Update Hud Hearts")]
    public static void Update()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var hud = GameObject.Find("Hud");
        if (hud == null)
        {
            Debug.LogError("VERIFY FAILED: Hud not found, build the HUD first");
            return;
        }

        RemoveIfPresent(hud.transform, "HealthBarBackground");
        RemoveIfPresent(hud.transform, "HealthBarFill");
        for (int i = 0; i < 6; i++)
        {
            RemoveIfPresent(hud.transform, $"Heart_{i}");
        }

        var fullSprite = FindSprite("heart_full");
        var emptySprite = FindSprite("heart_empty");
        if (fullSprite == null || emptySprite == null)
        {
            Debug.LogError("VERIFY FAILED: heart sprites not found, configure HeartIcons.png first");
            return;
        }

        var heartImages = new Image[HeartCount];
        for (int i = 0; i < HeartCount; i++)
        {
            var heart = new GameObject($"Heart_{i}", typeof(RectTransform));
            heart.transform.SetParent(hud.transform, false);

            var rect = (RectTransform)heart.transform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(20f + i * HeartSpacing, -20f);
            rect.sizeDelta = HeartSize;

            var image = heart.AddComponent<Image>();
            image.sprite = fullSprite;
            image.preserveAspect = true;

            heartImages[i] = image;
        }

        var controller = hud.GetComponent<HudController>();
        var so = new SerializedObject(controller);
        var heartsProp = so.FindProperty("heartIcons");
        heartsProp.arraySize = heartImages.Length;
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartsProp.GetArrayElementAtIndex(i).objectReferenceValue = heartImages[i];
        }
        so.FindProperty("heartFullSprite").objectReferenceValue = fullSprite;
        so.FindProperty("heartEmptySprite").objectReferenceValue = emptySprite;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void RemoveIfPresent(Transform parent, string name)
    {
        var child = parent.Find(name);
        if (child != null)
        {
            Object.DestroyImmediate(child.gameObject);
        }
    }

    private static Sprite FindSprite(string name)
    {
        var all = AssetDatabase.LoadAllAssetsAtPath(HeartSheetPath);
        foreach (var obj in all)
        {
            if (obj is Sprite s && s.name == name)
            {
                return s;
            }
        }
        return null;
    }

    private static void Verify()
    {
        var hud = GameObject.Find("Hud");
        var controller = hud.GetComponent<HudController>();
        var so = new SerializedObject(controller);
        var heartsProp = so.FindProperty("heartIcons");
        Debug.Log($"VERIFY: heartIcons count={heartsProp.arraySize}");
        for (int i = 0; i < heartsProp.arraySize; i++)
        {
            Debug.Log($"VERIFY: heart[{i}]={heartsProp.GetArrayElementAtIndex(i).objectReferenceValue?.name}");
        }
        Debug.Log($"VERIFY: heartFullSprite={so.FindProperty("heartFullSprite").objectReferenceValue?.name}");
        Debug.Log($"VERIFY: heartEmptySprite={so.FindProperty("heartEmptySprite").objectReferenceValue?.name}");
    }
}

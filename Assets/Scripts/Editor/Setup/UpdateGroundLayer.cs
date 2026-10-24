using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// Re-points the existing Ground parallax layer at the new bottom-pivot
/// ground texture and drops the old sandy tint to match the new art's own
/// color, without touching anything else already wired in the scene.
public static class UpdateGroundLayer
{
    private const string ScenePath = "Assets/Scenes/Game.unity";
    private const float GroundTopY = 0f;

    [MenuItem("Tools/Setup/Update Ground Layer")]
    public static void Update()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var ground = GameObject.Find("Ground");
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Environment/ground.png");
        if (ground == null || sprite == null)
        {
            Debug.LogError("VERIFY FAILED: Ground object or new ground sprite not found");
            return;
        }

        ground.transform.position = new Vector3(0f, GroundTopY, 0f);

        // ParallaxLayer rebuilds its segments fresh in Awake() but never
        // clears old ones, so any leftover children from the previous
        // ground sprite must be removed now to avoid doubled-up segments
        for (int i = ground.transform.childCount - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(ground.transform.GetChild(i).gameObject);
        }

        var layer = ground.GetComponent<ParallaxLayer>();
        var so = new SerializedObject(layer);
        so.FindProperty("sprite").objectReferenceValue = sprite;
        so.FindProperty("tint").colorValue = Color.white;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var ground = GameObject.Find("Ground");
        Debug.Log($"VERIFY: Ground pos={ground.transform.position}");
    }
}

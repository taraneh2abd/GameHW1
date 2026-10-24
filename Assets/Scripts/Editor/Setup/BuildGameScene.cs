using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildGameScene
{
    private const string OldScenePath = "Assets/Scenes/SampleScene.unity";
    private const string ScenePath = "Assets/Scenes/Game.unity";

    private const float CameraOrthoSize = 3f;
    private const float GroundTopY = 0f;

    [MenuItem("Tools/Setup/Build Game Scene")]
    public static void Build()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(OldScenePath) != null)
        {
            AssetDatabase.RenameAsset(OldScenePath, "Game");
            AssetDatabase.SaveAssets();
        }

        var scene = EditorSceneManager.OpenScene(ScenePath);

        ConfigureCamera();
        BuildGroundLayer();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true),
        };

        Verify();
    }

    private static void ConfigureCamera()
    {
        var cameraObj = GameObject.Find("Main Camera");
        if (cameraObj == null)
        {
            Debug.LogError("VERIFY FAILED: no Main Camera in scene");
            return;
        }

        var cam = cameraObj.GetComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = CameraOrthoSize;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.63f, 0.84f, 0.98f); // desert sky blue

        var t = cameraObj.transform;
        t.position = new Vector3(0f, 1f, t.position.z);
    }

    private static void BuildGroundLayer()
    {
        var existing = GameObject.Find("Ground");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        var groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Environment/ground.png");
        if (groundSprite == null)
        {
            Debug.LogError("VERIFY FAILED: ground sprite not found");
            return;
        }

        var ground = new GameObject("Ground");
        // sprite is a 5-unit-tall opaque block; its top edge is the stand line
        ground.transform.position = new Vector3(0f, GroundTopY - groundSprite.bounds.extents.y, 0f);

        var layer = ground.AddComponent<ParallaxLayer>();
        var so = new SerializedObject(layer);
        so.FindProperty("sprite").objectReferenceValue = groundSprite;
        so.FindProperty("speedScale").floatValue = 1f;
        so.FindProperty("sortingOrder").intValue = 0;
        so.FindProperty("segmentCount").intValue = 3;
        so.FindProperty("tint").colorValue = new Color(0.86f, 0.75f, 0.56f); // sandy tan
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void Verify()
    {
        var cam = Camera.main;
        Debug.Log($"VERIFY: camera orthoSize={cam.orthographicSize} pos={cam.transform.position} backgroundColor={cam.backgroundColor}");

        var ground = GameObject.Find("Ground");
        if (ground == null)
        {
            Debug.LogError("VERIFY FAILED: Ground object missing after save");
            return;
        }

        var layer = ground.GetComponent<ParallaxLayer>();
        Debug.Log($"VERIFY: Ground pos={ground.transform.position} hasParallaxLayer={layer != null}");

        var buildScenes = EditorBuildSettings.scenes;
        foreach (var s in buildScenes)
        {
            Debug.Log($"VERIFY: build scene path={s.path} enabled={s.enabled}");
        }
    }
}

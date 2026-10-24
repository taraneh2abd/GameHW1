using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public static class BuildPlayer
{
    private const string ScenePath = "Assets/Scenes/Game.unity";
    private const float GroundTopY = 0f;
    private const float GravityScale = 2.3f;

    [MenuItem("Tools/Setup/Build Player")]
    public static void Build()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var startSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Player/player_run_0.png");
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Player/Player.controller");
        var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Input/GameControls.inputactions");

        if (startSprite == null || controller == null || inputActions == null)
        {
            Debug.LogError("VERIFY FAILED: missing player sprite, controller, or input actions asset");
            return;
        }

        var existing = GameObject.Find("Player");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        var player = new GameObject("Player");
        float halfHeight = startSprite.bounds.extents.y;
        float groundY = GroundTopY + halfHeight;
        player.transform.position = new Vector3(0f, groundY, 0f);

        var spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = startSprite;
        spriteRenderer.sortingOrder = 10;

        var body = player.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = GravityScale;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var collider = player.AddComponent<BoxCollider2D>();
        collider.size = startSprite.bounds.size;

        var animator = player.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        var controllerComponent = player.AddComponent<PlayerController>();
        var so = new SerializedObject(controllerComponent);
        so.FindProperty("inputActions").objectReferenceValue = inputActions;
        so.FindProperty("groundY").floatValue = groundY;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("VERIFY FAILED: Player not found after save");
            return;
        }

        var controller = player.GetComponent<PlayerController>();
        var animator = player.GetComponent<Animator>();
        var body = player.GetComponent<Rigidbody2D>();
        var collider = player.GetComponent<BoxCollider2D>();

        Debug.Log($"VERIFY: Player pos={player.transform.position}");
        Debug.Log($"VERIFY: Rigidbody2D gravityScale={body.gravityScale} constraints={body.constraints}");
        Debug.Log($"VERIFY: BoxCollider2D size={collider.size}");
        Debug.Log($"VERIFY: Animator controller={(animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "NULL")}");
        Debug.Log($"VERIFY: PlayerController present={controller != null}");
    }
}

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// Swaps the already-wired Player GameObject's default sprite/collider to
/// match a new sprite source, without touching PlayerController/PlayerHealth
/// or anything that already references this GameObject (GameManager, etc).
public static class UpdatePlayerSprite
{
    private const string ScenePath = "Assets/Scenes/Game.unity";
    private const float GroundTopY = 0f;

    // Dino_Run01/02/Idle share a padded 96px canvas; the real visible height
    // (measured) is 94px, not the padded 96px sprite.bounds would report.
    private const float ContentHeightPx = 94f;
    private const float PixelsPerUnit = 90f;

    [MenuItem("Tools/Setup/Update Player Sprite")]
    public static void Update()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        var player = GameObject.Find("Player");
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Player/Dino_Run01.png");
        if (player == null || sprite == null)
        {
            Debug.LogError("VERIFY FAILED: Player or new default sprite not found");
            return;
        }

        float height = ContentHeightPx / PixelsPerUnit;

        var spriteRenderer = player.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

        var collider = player.GetComponent<BoxCollider2D>();
        collider.size = new Vector2(sprite.bounds.size.x, height);
        collider.offset = new Vector2(0f, height * 0.5f);

        // bottom-pivot sprite: the GameObject origin sits exactly on the ground
        player.transform.position = new Vector3(player.transform.position.x, GroundTopY, player.transform.position.z);

        var controller = player.GetComponent<PlayerController>();
        var so = new SerializedObject(controller);
        so.FindProperty("groundY").floatValue = GroundTopY;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static void Verify()
    {
        var player = GameObject.Find("Player");
        var spriteRenderer = player.GetComponent<SpriteRenderer>();
        var collider = player.GetComponent<BoxCollider2D>();
        var health = player.GetComponent<PlayerHealth>();
        var animator = player.GetComponent<Animator>();

        Debug.Log($"VERIFY: Player pos={player.transform.position} sprite={spriteRenderer.sprite.name} colliderSize={collider.size} colliderOffset={collider.offset}");
        Debug.Log($"VERIFY: PlayerHealth still present={health != null}, Animator controller still={(animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "NULL")}");
    }
}

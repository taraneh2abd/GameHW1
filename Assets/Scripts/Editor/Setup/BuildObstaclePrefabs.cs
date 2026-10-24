using UnityEditor;
using UnityEngine;

public static class BuildObstaclePrefabs
{
    private const string PrefabFolder = "Assets/Prefabs";
    private const float GroundTopY = 0f;
    private const float CactusVisualScale = 0.52f;
    private const float FlyingObstacleY = 1.3f;

    // the pterodactyl art has empty padding below the visible silhouette
    // within its frame, so the trigger is shrunk/offset to the actual
    // visible wingspan instead of the full (padded) sprite rect
    private static readonly Vector2 FlyingObstacleColliderSize = new Vector2(1f, 0.5f);
    private static readonly Vector2 FlyingObstacleColliderOffset = new Vector2(0f, 0.15f);

    [MenuItem("Tools/Setup/Build Obstacle Prefabs")]
    public static void Build()
    {
        EnsureFolder();
        BuildGroundObstacle();
        BuildFlyingObstacle();
        Verify();
    }

    private static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
    }

    private static void BuildGroundObstacle()
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Obstacles/cactus.png");
        if (sprite == null)
        {
            Debug.LogError("VERIFY FAILED: cactus sprite not found");
            return;
        }

        var go = new GameObject("GroundObstacle");
        go.transform.localScale = Vector3.one * CactusVisualScale;
        float y = GroundTopY + sprite.bounds.extents.y * CactusVisualScale;
        go.transform.position = new Vector3(0f, y, 0f);

        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 5;

        var collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = sprite.bounds.size;

        go.AddComponent<GroundObstacle>();

        SavePrefabAndDestroy(go, "GroundObstacle");
    }

    private static void BuildFlyingObstacle()
    {
        var frameA = FindSprite("Assets/Sprites/Obstacles/Pterodactylus.png", "pterodactylus_0");
        var frameB = FindSprite("Assets/Sprites/Obstacles/Pterodactylus.png", "pterodactylus_1");
        if (frameA == null || frameB == null)
        {
            Debug.LogError("VERIFY FAILED: flying obstacle frames not found");
            return;
        }

        var go = new GameObject("FlyingObstacle");
        go.transform.position = new Vector3(0f, FlyingObstacleY, 0f);

        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = frameA;
        spriteRenderer.sortingOrder = 5;

        var collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = FlyingObstacleColliderSize;
        collider.offset = FlyingObstacleColliderOffset;

        var flying = go.AddComponent<FlyingObstacle>();
        var so = new SerializedObject(flying);
        so.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
        so.FindProperty("frameA").objectReferenceValue = frameA;
        so.FindProperty("frameB").objectReferenceValue = frameB;
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefabAndDestroy(go, "FlyingObstacle");
    }

    private static void SavePrefabAndDestroy(GameObject go, string name)
    {
        var path = $"{PrefabFolder}/{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
    }

    private static Sprite FindSprite(string path, string name)
    {
        var all = AssetDatabase.LoadAllAssetsAtPath(path);
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
        string[] names = { "GroundObstacle", "FlyingObstacle" };
        foreach (var name in names)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/{name}.prefab");
            if (prefab == null)
            {
                Debug.LogError($"VERIFY FAILED: {name}.prefab missing");
                continue;
            }

            var renderer = prefab.GetComponent<SpriteRenderer>();
            var collider = prefab.GetComponent<BoxCollider2D>();
            Debug.Log($"VERIFY: {name} pos={prefab.transform.position} scale={prefab.transform.localScale} sprite={(renderer != null ? renderer.sprite.name : "NULL")} colliderSize={(collider != null ? collider.size.ToString() : "NULL")} isTrigger={(collider != null ? collider.isTrigger.ToString() : "NULL")}");
        }
    }
}

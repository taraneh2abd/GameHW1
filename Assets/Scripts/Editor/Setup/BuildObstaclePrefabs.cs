using UnityEditor;
using UnityEngine;

public static class BuildObstaclePrefabs
{
    private const string PrefabFolder = "Assets/Prefabs";
    private const float GroundTopY = 0f;
    private const float FlyingObstacleY = 1.35f;

    // the bird art has empty padding below the visible wingspan within its
    // frame, so the trigger is shrunk/offset to the actual silhouette
    // instead of the full (padded) sprite rect
    private static readonly Vector2 FlyingObstacleColliderSize = new Vector2(1f, 0.4f);
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
        var variants = new[]
        {
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Obstacles/cactus_medium.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Obstacles/cactus_tall_a.png"),
            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Obstacles/cactus_tall_b.png"),
        };
        foreach (var variant in variants)
        {
            if (variant == null)
            {
                Debug.LogError("VERIFY FAILED: a cactus variant sprite is missing");
                return;
            }
        }

        var defaultSprite = variants[1];

        var go = new GameObject("GroundObstacle");
        float y = GroundTopY + defaultSprite.bounds.extents.y;
        go.transform.position = new Vector3(0f, y, 0f);

        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.sortingOrder = 5;

        var collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = defaultSprite.bounds.size;

        var obstacle = go.AddComponent<GroundObstacle>();
        var so = new SerializedObject(obstacle);
        so.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
        so.FindProperty("boxCollider").objectReferenceValue = collider;
        var variantsProp = so.FindProperty("variants");
        variantsProp.arraySize = variants.Length;
        for (int i = 0; i < variants.Length; i++)
        {
            variantsProp.GetArrayElementAtIndex(i).objectReferenceValue = variants[i];
        }
        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefabAndDestroy(go, "GroundObstacle");
    }

    private static void BuildFlyingObstacle()
    {
        var frameA = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Obstacles/bird_0.png");
        var frameB = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Obstacles/bird_1.png");
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

    private static void Verify()
    {
        var ground = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/GroundObstacle.prefab");
        if (ground != null)
        {
            var obstacle = ground.GetComponent<GroundObstacle>();
            var so = new SerializedObject(obstacle);
            var variantsProp = so.FindProperty("variants");
            Debug.Log($"VERIFY: GroundObstacle pos={ground.transform.position} variantCount={variantsProp.arraySize}");
            for (int i = 0; i < variantsProp.arraySize; i++)
            {
                Debug.Log($"VERIFY: GroundObstacle variant[{i}]={variantsProp.GetArrayElementAtIndex(i).objectReferenceValue?.name}");
            }
        }
        else
        {
            Debug.LogError("VERIFY FAILED: GroundObstacle.prefab missing");
        }

        var flying = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/FlyingObstacle.prefab");
        if (flying != null)
        {
            var collider = flying.GetComponent<BoxCollider2D>();
            var renderer = flying.GetComponent<SpriteRenderer>();
            Debug.Log($"VERIFY: FlyingObstacle pos={flying.transform.position} sprite={renderer.sprite.name} colliderSize={collider.size} colliderOffset={collider.offset}");
        }
        else
        {
            Debug.LogError("VERIFY FAILED: FlyingObstacle.prefab missing");
        }
    }
}

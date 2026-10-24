using UnityEditor;
using UnityEngine;

public static class BuildObstaclePrefabs
{
    private const string PrefabFolder = "Assets/Prefabs";
    private const float GroundTopY = 0f;
    // sits in the upper half of the player's standing height: a standing
    // player clips it, a ducking player (shrunk collider) passes underneath
    private const float FlyingObstacleY = 0.78f;
    private static readonly Vector2 FlyingObstacleColliderSize = new Vector2(0.9f, 0.45f);
    private static readonly Vector2 FlyingObstacleColliderOffset = Vector2.zero;

    private struct CactusVariant
    {
        public string FileName;
        public float Height; // real visible height in pixels, not the shared padded canvas

        public CactusVariant(string fileName, float height)
        {
            FileName = fileName;
            Height = height;
        }
    }

    private static readonly CactusVariant[] CactusVariants =
    {
        new CactusVariant("Cactus_Large_Single", 96f),
        new CactusVariant("Cactus_Large_Double", 96f),
        new CactusVariant("Cactus_Large_Triple", 96f),
        new CactusVariant("Cactus_Small_Single", 70f),
        new CactusVariant("Cactus_Small_Double", 70f),
        new CactusVariant("Cactus_Small_Triple", 70f),
    };

    private const float PixelsPerUnit = 90f;

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
        var sprites = new Sprite[CactusVariants.Length];
        var heights = new float[CactusVariants.Length];
        for (int i = 0; i < CactusVariants.Length; i++)
        {
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/Obstacles/{CactusVariants[i].FileName}.png");
            if (sprite == null)
            {
                Debug.LogError($"VERIFY FAILED: missing cactus sprite {CactusVariants[i].FileName}");
                return;
            }
            sprites[i] = sprite;
            heights[i] = CactusVariants[i].Height / PixelsPerUnit;
        }

        var defaultSprite = sprites[0];
        float defaultHeight = heights[0];

        var go = new GameObject("GroundObstacle");
        go.transform.position = new Vector3(0f, GroundTopY, 0f);

        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.sortingOrder = 5;

        var collider = go.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = new Vector2(defaultSprite.bounds.size.x, defaultHeight);
        collider.offset = new Vector2(0f, defaultHeight * 0.5f);

        var obstacle = go.AddComponent<GroundObstacle>();
        var so = new SerializedObject(obstacle);
        so.FindProperty("spriteRenderer").objectReferenceValue = spriteRenderer;
        so.FindProperty("boxCollider").objectReferenceValue = collider;

        var variantsProp = so.FindProperty("variants");
        variantsProp.arraySize = sprites.Length;
        for (int i = 0; i < sprites.Length; i++)
        {
            variantsProp.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
        }

        var heightsProp = so.FindProperty("variantHeights");
        heightsProp.arraySize = heights.Length;
        for (int i = 0; i < heights.Length; i++)
        {
            heightsProp.GetArrayElementAtIndex(i).floatValue = heights[i];
        }

        so.ApplyModifiedPropertiesWithoutUndo();

        SavePrefabAndDestroy(go, "GroundObstacle");
    }

    private static void BuildFlyingObstacle()
    {
        var frameA = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Obstacles/Bird_01.png");
        var frameB = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Obstacles/Bird_02.png");
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
            var heightsProp = so.FindProperty("variantHeights");
            Debug.Log($"VERIFY: GroundObstacle pos={ground.transform.position} variantCount={variantsProp.arraySize}");
            for (int i = 0; i < variantsProp.arraySize; i++)
            {
                Debug.Log($"VERIFY: variant[{i}]={variantsProp.GetArrayElementAtIndex(i).objectReferenceValue?.name} height={heightsProp.GetArrayElementAtIndex(i).floatValue}");
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

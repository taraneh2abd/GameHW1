using UnityEditor;
using UnityEngine;

public static class BuildCoinPrefab
{
    private const string PrefabPath = "Assets/Prefabs/Coin.prefab";
    private const float CoinY = 0.6f;

    [MenuItem("Tools/Setup/Build Coin Prefab")]
    public static void Build()
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Items/coin.png");
        if (sprite == null)
        {
            Debug.LogError("VERIFY FAILED: coin sprite not found");
            return;
        }

        var go = new GameObject("Coin");
        go.transform.position = new Vector3(0f, CoinY, 0f);

        var spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = new Color(1f, 0.84f, 0.2f); // gold
        spriteRenderer.sortingOrder = 5;

        var collider = go.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = sprite.bounds.extents.x;

        go.AddComponent<Coin>();

        PrefabUtility.SaveAsPrefabAsset(go, PrefabPath);
        Object.DestroyImmediate(go);

        Verify();
    }

    private static void Verify()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError("VERIFY FAILED: Coin.prefab missing");
            return;
        }

        var renderer = prefab.GetComponent<SpriteRenderer>();
        var collider = prefab.GetComponent<CircleCollider2D>();
        Debug.Log($"VERIFY: Coin pos={prefab.transform.position} color={renderer.color} colliderRadius={collider.radius} isTrigger={collider.isTrigger}");
    }
}

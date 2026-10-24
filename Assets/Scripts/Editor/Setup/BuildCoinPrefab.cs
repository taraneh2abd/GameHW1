using UnityEditor;
using UnityEngine;

public static class BuildCoinPrefab
{
    private const string PrefabPath = "Assets/Prefabs/Coin.prefab";
    private const string AsepritePath = "Assets/Sprites/Items/coin_spin.aseprite";
    private const float CoinY = 0.3f;

    // native aseprite import is tiny (8px frames at the importer's default
    // 100 px/unit); scale the instance up instead of touching import settings
    private const float VisualScale = 6.25f;

    [MenuItem("Tools/Setup/Build Coin Prefab")]
    public static void Build()
    {
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(AsepritePath);
        if (source == null)
        {
            Debug.LogError("VERIFY FAILED: coin_spin.aseprite root GameObject not found");
            return;
        }

        var go = (GameObject)Object.Instantiate(source);
        go.name = "Coin";
        go.transform.position = new Vector3(0f, CoinY, 0f);
        go.transform.localScale = Vector3.one * VisualScale;

        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("VERIFY FAILED: generated coin GameObject has no SpriteRenderer");
            return;
        }
        spriteRenderer.sortingOrder = 5;

        var collider = go.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.25f;

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
        var animator = prefab.GetComponent<Animator>();
        var coin = prefab.GetComponent<Coin>();
        Debug.Log($"VERIFY: Coin pos={prefab.transform.position} scale={prefab.transform.localScale} colliderRadius={collider.radius} isTrigger={collider.isTrigger}");
        Debug.Log($"VERIFY: has Animator={animator != null} controller={(animator != null && animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "NULL")} has Coin script={coin != null}");
    }
}

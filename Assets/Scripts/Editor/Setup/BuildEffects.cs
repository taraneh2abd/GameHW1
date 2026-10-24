using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildEffects
{
    private const string ScenePath = "Assets/Scenes/Game.unity";
    private const string PrefabFolder = "Assets/Prefabs";

    [MenuItem("Tools/Setup/Build Effects")]
    public static void Build()
    {
        var coinPop = BuildBurstPrefab("CoinPopEffect", new Color(1f, 0.84f, 0.2f), 10, 2f);
        var hitImpact = BuildBurstPrefab("HitImpactEffect", new Color(0.9f, 0.2f, 0.2f), 14, 2.5f);
        var dustPuff = BuildBurstPrefab("DustPuffEffect", new Color(0.6f, 0.55f, 0.45f), 6, 1.2f);

        var scene = EditorSceneManager.OpenScene(ScenePath);

        var mainCameraObj = GameObject.Find("Main Camera");
        if (mainCameraObj == null)
        {
            Debug.LogError("VERIFY FAILED: Main Camera not found");
            return;
        }

        var shake = mainCameraObj.GetComponent<CameraShake>();
        if (shake == null)
        {
            shake = mainCameraObj.AddComponent<CameraShake>();
        }

        var existingEffects = GameObject.Find("Effects");
        if (existingEffects != null)
        {
            Object.DestroyImmediate(existingEffects);
        }

        var effectsObj = new GameObject("Effects");
        var controller = effectsObj.AddComponent<EffectsController>();
        var so = new SerializedObject(controller);
        so.FindProperty("coinPopPrefab").objectReferenceValue = coinPop;
        so.FindProperty("hitImpactPrefab").objectReferenceValue = hitImpact;
        so.FindProperty("cameraShake").objectReferenceValue = shake;
        so.ApplyModifiedPropertiesWithoutUndo();

        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("VERIFY FAILED: Player not found");
            return;
        }
        var playerController = player.GetComponent<PlayerController>();
        var playerSo = new SerializedObject(playerController);
        playerSo.FindProperty("dustPuffPrefab").objectReferenceValue = dustPuff;
        playerSo.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Verify();
    }

    private static GameObject BuildBurstPrefab(string name, Color color, int count, float speed)
    {
        var path = $"{PrefabFolder}/{name}.prefab";

        var go = new GameObject(name);
        go.AddComponent<ParticleSystem>();
        var burst = go.AddComponent<BurstEffect>();

        var so = new SerializedObject(burst);
        so.FindProperty("color").colorValue = color;
        so.FindProperty("particleCount").intValue = count;
        so.FindProperty("speed").floatValue = speed;
        so.ApplyModifiedPropertiesWithoutUndo();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static void Verify()
    {
        string[] names = { "CoinPopEffect", "HitImpactEffect", "DustPuffEffect" };
        foreach (var name in names)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/{name}.prefab");
            Debug.Log($"VERIFY: {name} exists={prefab != null} hasParticleSystem={(prefab != null && prefab.GetComponent<ParticleSystem>() != null)}");
        }

        var effects = GameObject.Find("Effects");
        var controller = effects != null ? effects.GetComponent<EffectsController>() : null;
        if (controller != null)
        {
            var so = new SerializedObject(controller);
            Debug.Log($"VERIFY: EffectsController coinPop={so.FindProperty("coinPopPrefab").objectReferenceValue?.name} hitImpact={so.FindProperty("hitImpactPrefab").objectReferenceValue?.name} cameraShake={so.FindProperty("cameraShake").objectReferenceValue?.name}");
        }
        else
        {
            Debug.LogError("VERIFY FAILED: EffectsController missing");
        }

        var player = GameObject.Find("Player");
        var playerController = player != null ? player.GetComponent<PlayerController>() : null;
        if (playerController != null)
        {
            var so = new SerializedObject(playerController);
            Debug.Log($"VERIFY: PlayerController dustPuffPrefab={so.FindProperty("dustPuffPrefab").objectReferenceValue?.name}");
        }
    }
}

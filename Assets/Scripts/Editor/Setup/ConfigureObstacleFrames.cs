using UnityEditor;
using UnityEngine;

public static class ConfigureObstacleFrames
{
    private const int PixelsPerUnit = 75;

    private static readonly string[] Names =
    {
        "bird_0", "bird_1", "cactus_medium", "cactus_tall_a", "cactus_tall_b",
    };

    [MenuItem("Tools/Setup/Configure Obstacle Frames")]
    public static void Configure()
    {
        foreach (var name in Names)
        {
            var path = $"Assets/Sprites/Obstacles/{name}.png";
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer == null)
            {
                Debug.LogError($"VERIFY FAILED: no importer for {path}");
                continue;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PixelsPerUnit;
            importer.filterMode = FilterMode.Bilinear;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;

            var settings = importer.GetDefaultPlatformTextureSettings();
            settings.maxTextureSize = 2048;
            settings.format = TextureImporterFormat.RGBA32;
            importer.SetPlatformTextureSettings(settings);

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Verify();
    }

    private static void Verify()
    {
        foreach (var name in Names)
        {
            var path = $"Assets/Sprites/Obstacles/{name}.png";
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogError($"VERIFY FAILED: no sprite loaded at {path}");
                continue;
            }
            Debug.Log($"VERIFY: {name} bounds={sprite.bounds.size} ppu={sprite.pixelsPerUnit}");
        }
    }
}

using UnityEditor;
using UnityEngine;

public static class ConfigurePlayerFrames
{
    private const int PixelsPerUnit = 90;

    private static readonly string[] FrameNames =
    {
        "player_run_0", "player_run_1", "player_run_2",
        "player_run_3", "player_run_4", "player_run_5",
        "player_jump",
    };

    [MenuItem("Tools/Setup/Configure Player Frames")]
    public static void Configure()
    {
        foreach (var name in FrameNames)
        {
            var path = $"Assets/Sprites/Player/{name}.png";
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
        foreach (var name in FrameNames)
        {
            var path = $"Assets/Sprites/Player/{name}.png";
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                Debug.LogError($"VERIFY FAILED: no sprite loaded at {path}");
                continue;
            }
            Debug.Log($"VERIFY: {name} rect={sprite.rect} ppu={sprite.pixelsPerUnit} bounds={sprite.bounds.size}");
        }
    }
}

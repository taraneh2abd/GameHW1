using UnityEditor;
using UnityEngine;

public static class ConfigureDinoAssets
{
    private const int PixelsPerUnit = 90;

    private static readonly string[] BottomPivotPaths =
    {
        "Assets/Sprites/Player/Dino_Run01.png",
        "Assets/Sprites/Player/Dino_Run02.png",
        "Assets/Sprites/Player/Dino_Idle.png",
        "Assets/Sprites/Player/Dino_Duck01.png",
        "Assets/Sprites/Player/Dino_Duck02.png",
        "Assets/Sprites/Obstacles/Cactus_Large_Single.png",
        "Assets/Sprites/Obstacles/Cactus_Large_Double.png",
        "Assets/Sprites/Obstacles/Cactus_Large_Triple.png",
        "Assets/Sprites/Obstacles/Cactus_Small_Single.png",
        "Assets/Sprites/Obstacles/Cactus_Small_Double.png",
        "Assets/Sprites/Obstacles/Cactus_Small_Triple.png",
        "Assets/Sprites/Environment/ground.png",
    };

    private static readonly string[] CenterPivotPaths =
    {
        "Assets/Sprites/Obstacles/Bird_01.png",
        "Assets/Sprites/Obstacles/Bird_02.png",
    };

    [MenuItem("Tools/Setup/Configure Dino Assets")]
    public static void Configure()
    {
        foreach (var path in BottomPivotPaths)
        {
            Configure(path, SpriteAlignment.BottomCenter);
        }
        foreach (var path in CenterPivotPaths)
        {
            Configure(path, SpriteAlignment.Center);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Verify();
    }

    private static void Configure(string path, SpriteAlignment alignment)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer == null)
        {
            Debug.LogError($"VERIFY FAILED: no importer for {path}");
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = PixelsPerUnit;

        var textureSettings = new TextureImporterSettings();
        importer.ReadTextureSettings(textureSettings);
        textureSettings.spriteAlignment = (int)alignment;
        textureSettings.spritePivot = alignment == SpriteAlignment.BottomCenter ? new Vector2(0.5f, 0f) : new Vector2(0.5f, 0.5f);
        importer.SetTextureSettings(textureSettings);

        importer.filterMode = FilterMode.Bilinear;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        var settings = importer.GetDefaultPlatformTextureSettings();
        settings.maxTextureSize = 4096;
        settings.format = TextureImporterFormat.RGBA32;
        importer.SetPlatformTextureSettings(settings);

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    private static void Verify()
    {
        foreach (var path in BottomPivotPaths)
        {
            LogSprite(path);
        }
        foreach (var path in CenterPivotPaths)
        {
            LogSprite(path);
        }
    }

    private static void LogSprite(string path)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogError($"VERIFY FAILED: no sprite loaded at {path}");
            return;
        }
        Debug.Log($"VERIFY: {path} bounds={sprite.bounds.size} pivot={sprite.pivot} ppu={sprite.pixelsPerUnit}");
    }
}

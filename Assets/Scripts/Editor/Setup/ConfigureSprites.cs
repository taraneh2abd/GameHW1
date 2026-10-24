using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public static class ConfigureSprites
{
    private const int PixelsPerUnit = 20;

    private struct SheetSpec
    {
        public string Path;
        public int FrameWidth;
        public int FrameHeight;
        public string BaseName;

        public SheetSpec(string path, int frameWidth, int frameHeight, string baseName)
        {
            Path = path;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            BaseName = baseName;
        }
    }

    [MenuItem("Tools/Setup/Configure Sprites")]
    public static void Configure()
    {
        var sheets = new[]
        {
            new SheetSpec("Assets/Sprites/Player/player.png", 20, 21, "player"),
            new SheetSpec("Assets/Sprites/Obstacles/Pterodactylus.png", 20, 18, "pterodactylus"),
        };

        foreach (var sheet in sheets)
        {
            ConfigureSingle(sheet.Path, SpriteImportMode.Multiple);
            SliceSheet(sheet.Path, sheet.FrameWidth, sheet.FrameHeight, sheet.BaseName);
        }

        ConfigureSingle("Assets/Sprites/Obstacles/cactus.png", SpriteImportMode.Single);
        ConfigureSingle("Assets/Sprites/Environment/ground.png", SpriteImportMode.Single);
        ConfigureSingle("Assets/Sprites/Items/coin.png", SpriteImportMode.Single);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Verify();
    }

    private static void ConfigureSingle(string path, SpriteImportMode mode)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer == null)
        {
            Debug.LogError($"VERIFY FAILED: no importer for {path}");
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = mode;
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

    private static void SliceSheet(string path, int frameWidth, int frameHeight, string baseName)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (importer == null || texture == null)
        {
            Debug.LogError($"VERIFY FAILED: could not load texture/importer for {path}");
            return;
        }

        int columns = texture.width / frameWidth;
        var dataProviderFactory = new SpriteDataProviderFactories();
        dataProviderFactory.Init();
        var dataProvider = dataProviderFactory.GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();

        var rects = new List<SpriteRect>();
        for (int i = 0; i < columns; i++)
        {
            rects.Add(new SpriteRect
            {
                name = $"{baseName}_{i}",
                rect = new Rect(i * frameWidth, 0, frameWidth, frameHeight),
                alignment = SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f),
            });
        }

        dataProvider.SetSpriteRects(rects.ToArray());
        dataProvider.Apply();

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    private static void Verify()
    {
        string[] paths =
        {
            "Assets/Sprites/Player/player.png",
            "Assets/Sprites/Obstacles/Pterodactylus.png",
            "Assets/Sprites/Obstacles/cactus.png",
            "Assets/Sprites/Environment/ground.png",
            "Assets/Sprites/Items/coin.png",
        };

        foreach (var path in paths)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path);
            int count = 0;
            foreach (var obj in sprites)
            {
                if (obj is Sprite sprite)
                {
                    count++;
                    Debug.Log($"VERIFY: {path} -> sprite '{sprite.name}' rect={sprite.rect} ppu={sprite.pixelsPerUnit}");
                }
            }

            if (count == 0)
            {
                Debug.LogError($"VERIFY FAILED: no sprites found at {path}");
            }
        }
    }
}

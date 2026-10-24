using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public static class ConfigureHeartIcons
{
    private const string Path = "Assets/Sprites/UI/HeartIcons.png";
    private const int FrameSize = 32;

    [MenuItem("Tools/Setup/Configure Heart Icons")]
    public static void Configure()
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(Path);
        if (importer == null)
        {
            Debug.LogError($"VERIFY FAILED: no importer for {Path}");
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.filterMode = FilterMode.Point;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        var settings = importer.GetDefaultPlatformTextureSettings();
        settings.maxTextureSize = 2048;
        settings.format = TextureImporterFormat.RGBA32;
        importer.SetPlatformTextureSettings(settings);

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(Path);
        int columns = texture.width / FrameSize;

        var factory = new SpriteDataProviderFactories();
        factory.Init();
        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();

        var names = new[] { "heart_full", "heart_full_alt", "heart_empty" };
        var rects = new List<SpriteRect>();
        for (int i = 0; i < columns; i++)
        {
            rects.Add(new SpriteRect
            {
                name = i < names.Length ? names[i] : $"heart_{i}",
                rect = new Rect(i * FrameSize, 0, FrameSize, FrameSize),
                alignment = SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f),
            });
        }

        dataProvider.SetSpriteRects(rects.ToArray());
        dataProvider.Apply();

        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Verify();
    }

    private static void Verify()
    {
        var sprites = AssetDatabase.LoadAllAssetsAtPath(Path);
        foreach (var obj in sprites)
        {
            if (obj is Sprite sprite)
            {
                Debug.Log($"VERIFY: sprite={sprite.name} rect={sprite.rect}");
            }
        }
    }
}

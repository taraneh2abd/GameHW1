using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public static class CreateInputActions
{
    private const string AssetPath = "Assets/Input/GameControls.inputactions";

    [MenuItem("Tools/Setup/Create Input Actions")]
    public static void Create()
    {
        var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        var gameplay = asset.AddActionMap("Gameplay");

        gameplay.AddAction("Jump", InputActionType.Button)
            .AddBinding("<Keyboard>/space");
        gameplay.AddAction("Dash", InputActionType.Button)
            .AddBinding("<Keyboard>/leftShift");
        gameplay.AddAction("Pause", InputActionType.Button)
            .AddBinding("<Keyboard>/escape");

        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), AssetPath);
        File.WriteAllText(fullPath, asset.ToJson());
        Object.DestroyImmediate(asset);

        AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        Verify();
    }

    private static void Verify()
    {
        var reloaded = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
        if (reloaded == null)
        {
            Debug.LogError($"VERIFY FAILED: could not reload {AssetPath}");
            return;
        }

        foreach (var map in reloaded.actionMaps)
        {
            foreach (var action in map.actions)
            {
                var bindingPaths = "";
                foreach (var binding in action.bindings)
                {
                    bindingPaths += binding.effectivePath + " ";
                }
                Debug.Log($"VERIFY: map={map.name} action={action.name} type={action.type} bindings=[{bindingPaths.Trim()}]");
            }
        }
    }
}

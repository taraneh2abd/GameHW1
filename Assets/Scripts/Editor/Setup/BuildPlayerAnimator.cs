using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class BuildPlayerAnimator
{
    private const string FolderPath = "Assets/Animations/Player";
    private const string ControllerPath = FolderPath + "/Player.controller";
    private const string SpriteFolder = "Assets/Sprites/Player";

    [MenuItem("Tools/Setup/Build Player Animator")]
    public static void Build()
    {
        EnsureFolder();

        var runFrames = new[] { LoadSprite("Dino_Run01"), LoadSprite("Dino_Run02") };
        var idleSprite = LoadSprite("Dino_Idle");
        var duckFrames = new[] { LoadSprite("Dino_Duck01"), LoadSprite("Dino_Duck02") };

        if (System.Array.IndexOf(runFrames, null) >= 0 || idleSprite == null || System.Array.IndexOf(duckFrames, null) >= 0)
        {
            Debug.LogError("VERIFY FAILED: missing one or more player sprites");
            return;
        }

        var runClip = CreateLoopingClip("Run", runFrames, 8f);
        var jumpClip = CreateStaticClip("Jump", idleSprite, 0.2f);
        var hitClip = CreateStaticClip("Hit", runFrames[0], 0.15f);
        var deathClip = CreateStaticClip("Death", idleSprite, 0.2f);
        var duckClip = CreateLoopingClip("Duck", duckFrames, 8f);

        if (AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath) != null)
        {
            AssetDatabase.DeleteAsset(ControllerPath);
        }

        var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
        controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Die", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Dead", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Ducking", AnimatorControllerParameterType.Bool);

        var stateMachine = controller.layers[0].stateMachine;

        var runState = stateMachine.AddState("Run");
        runState.motion = runClip;
        stateMachine.defaultState = runState;

        var jumpState = stateMachine.AddState("Jump");
        jumpState.motion = jumpClip;

        var hitState = stateMachine.AddState("Hit");
        hitState.motion = hitClip;

        var deathState = stateMachine.AddState("Death");
        deathState.motion = deathClip;

        var duckState = stateMachine.AddState("Duck");
        duckState.motion = duckClip;

        var runToJump = runState.AddTransition(jumpState);
        runToJump.hasExitTime = false;
        runToJump.duration = 0f;
        runToJump.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");

        var jumpToRun = jumpState.AddTransition(runState);
        jumpToRun.hasExitTime = false;
        jumpToRun.duration = 0f;
        jumpToRun.AddCondition(AnimatorConditionMode.If, 0, "Grounded");

        var runToDuck = runState.AddTransition(duckState);
        runToDuck.hasExitTime = false;
        runToDuck.duration = 0f;
        runToDuck.AddCondition(AnimatorConditionMode.If, 0, "Ducking");

        var duckToRun = duckState.AddTransition(runState);
        duckToRun.hasExitTime = false;
        duckToRun.duration = 0f;
        duckToRun.AddCondition(AnimatorConditionMode.IfNot, 0, "Ducking");

        var anyToHit = stateMachine.AddAnyStateTransition(hitState);
        anyToHit.hasExitTime = false;
        anyToHit.duration = 0f;
        anyToHit.canTransitionToSelf = false;
        anyToHit.AddCondition(AnimatorConditionMode.If, 0, "Hit");
        anyToHit.AddCondition(AnimatorConditionMode.IfNot, 0, "Dead");

        var hitToRun = hitState.AddTransition(runState);
        hitToRun.hasExitTime = true;
        hitToRun.exitTime = 1f;
        hitToRun.duration = 0f;

        var anyToDeath = stateMachine.AddAnyStateTransition(deathState);
        anyToDeath.hasExitTime = false;
        anyToDeath.duration = 0f;
        anyToDeath.canTransitionToSelf = false;
        anyToDeath.AddCondition(AnimatorConditionMode.If, 0, "Die");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Verify();
    }

    private static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Animations"))
        {
            AssetDatabase.CreateFolder("Assets", "Animations");
        }
        if (!AssetDatabase.IsValidFolder(FolderPath))
        {
            AssetDatabase.CreateFolder("Assets/Animations", "Player");
        }
    }

    private static Sprite LoadSprite(string name)
    {
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{SpriteFolder}/{name}.png");
        if (sprite == null)
        {
            Debug.LogError($"VERIFY FAILED: missing sprite {name}");
        }
        return sprite;
    }

    private static AnimationClip CreateLoopingClip(string name, Sprite[] frames, float fps)
    {
        var clip = new AnimationClip { frameRate = fps };
        var binding = new EditorCurveBinding { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" };

        var keyframes = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < frames.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe { time = i / fps, value = frames[i] };
        }
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        var path = $"{FolderPath}/{name}.anim";
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static AnimationClip CreateStaticClip(string name, Sprite sprite, float duration)
    {
        var clip = new AnimationClip { frameRate = 10f };
        var binding = new EditorCurveBinding { path = "", type = typeof(SpriteRenderer), propertyName = "m_Sprite" };

        var keyframes = new[]
        {
            new ObjectReferenceKeyframe { time = 0f, value = sprite },
            new ObjectReferenceKeyframe { time = duration, value = sprite },
        };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        var path = $"{FolderPath}/{name}.anim";
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static void Verify()
    {
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            Debug.LogError($"VERIFY FAILED: could not reload {ControllerPath}");
            return;
        }

        Debug.Log($"VERIFY: parameters={controller.parameters.Length}");
        var sm = controller.layers[0].stateMachine;
        Debug.Log($"VERIFY: states={sm.states.Length} defaultState={sm.defaultState.name}");
        foreach (var s in sm.states)
        {
            Debug.Log($"VERIFY: state={s.state.name} motion={(s.state.motion != null ? s.state.motion.name : "NULL")} outgoingTransitions={s.state.transitions.Length}");
        }
        Debug.Log($"VERIFY: anyStateTransitions={sm.anyStateTransitions.Length}");
    }
}

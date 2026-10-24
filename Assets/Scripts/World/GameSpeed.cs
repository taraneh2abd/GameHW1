using UnityEngine;

/// Shared world-scroll speed (world units/second) that the ground, parallax
/// layers, and obstacles all move by, so difficulty ramping and dashing only
/// need to touch one place.
public static class GameSpeed
{
    public const float DefaultBaseSpeed = 6f;

    public static float BaseSpeed { get; private set; } = DefaultBaseSpeed;
    public static float DashMultiplier { get; set; } = 1f;

    public static float Current => BaseSpeed * DashMultiplier;

    public static void Reset()
    {
        BaseSpeed = DefaultBaseSpeed;
        DashMultiplier = 1f;
    }

    public static void SetBaseSpeed(float value)
    {
        BaseSpeed = Mathf.Max(0f, value);
    }
}

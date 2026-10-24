using UnityEngine;

/// Shared world-scroll speed (world units/second) that the ground, parallax
/// layers, and obstacles all move by, so difficulty ramping only needs to
/// touch one place.
public static class GameSpeed
{
    public const float BaseSpeed = 6f;

    public static float Current { get; private set; } = BaseSpeed;

    public static void Reset()
    {
        Current = BaseSpeed;
    }

    public static void SetCurrent(float value)
    {
        Current = Mathf.Max(0f, value);
    }
}

using System;
using UnityEngine;

/// Tracks the current run's score and the persisted high score. A plain
/// static class so UI and gameplay code can read/update it without needing
/// a scene reference.
public static class ScoreManager
{
    private const string HighScoreKey = "HighScore";
    private const int CoinScoreValue = 10;
    private const float PointsPerSecond = 10f;

    public static int CurrentScore { get; private set; }
    public static int HighScore { get; private set; }

    public static event Action<int, int> ScoreChanged;

    public static void Reset()
    {
        CurrentScore = 0;
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        ScoreChanged?.Invoke(CurrentScore, HighScore);
    }

    public static void AddElapsedTime(float deltaTime)
    {
        AddScore(Mathf.RoundToInt(PointsPerSecond * deltaTime));
    }

    public static void AddCoinScore()
    {
        AddScore(CoinScoreValue);
    }

    private static void AddScore(int amount)
    {
        if (amount == 0)
        {
            return;
        }

        CurrentScore += amount;
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
        }
        ScoreChanged?.Invoke(CurrentScore, HighScore);
    }
}

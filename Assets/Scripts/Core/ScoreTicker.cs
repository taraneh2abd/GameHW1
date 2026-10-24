using UnityEngine;

/// Drives ScoreManager from the scene: ticks score up over time and listens
/// for coin pickups. Keeps ScoreManager itself a plain static class.
public class ScoreTicker : MonoBehaviour
{
    private void OnEnable()
    {
        ScoreManager.Reset();
        Coin.Collected += OnCoinCollected;
    }

    private void OnDisable()
    {
        Coin.Collected -= OnCoinCollected;
    }

    private void Update()
    {
        ScoreManager.AddElapsedTime(Time.deltaTime);
    }

    private void OnCoinCollected(Vector3 position)
    {
        ScoreManager.AddCoinScore();
    }
}

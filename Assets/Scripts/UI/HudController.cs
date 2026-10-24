using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Text currentScoreText;
    [SerializeField] private Text highScoreText;

    private void OnEnable()
    {
        PlayerHealth.LivesChanged += OnLivesChanged;
        ScoreManager.ScoreChanged += OnScoreChanged;

        OnScoreChanged(ScoreManager.CurrentScore, ScoreManager.HighScore);

        var playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            OnLivesChanged(playerHealth.Lives, playerHealth.MaxLives);
        }
    }

    private void OnDisable()
    {
        PlayerHealth.LivesChanged -= OnLivesChanged;
        ScoreManager.ScoreChanged -= OnScoreChanged;
    }

    private void OnLivesChanged(int lives, int maxLives)
    {
        healthFill.fillAmount = maxLives > 0 ? (float)lives / maxLives : 0f;
    }

    private void OnScoreChanged(int current, int high)
    {
        currentScoreText.text = $"Score: {current}";
        highScoreText.text = $"Best: {high}";
    }
}

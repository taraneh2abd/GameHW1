using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [SerializeField] private Image[] heartIcons;
    [SerializeField] private Sprite heartFullSprite;
    [SerializeField] private Sprite heartEmptySprite;
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
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].sprite = i < lives ? heartFullSprite : heartEmptySprite;
        }
    }

    private void OnScoreChanged(int current, int high)
    {
        currentScoreText.text = $"Score: {current}";
        highScoreText.text = $"Best: {high}";
    }
}

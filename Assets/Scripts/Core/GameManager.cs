using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text finalScoreText;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject obstacleSpawner;
    [SerializeField] private GameObject scoreTicker;

    private static bool autoStartOnLoad;

    private InputAction pauseAction;

    public GameState State { get; private set; }

    private void Awake()
    {
        pauseAction = inputActions.FindActionMap("Gameplay").FindAction("Pause");
    }

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePressed;
        PlayerHealth.Died += OnPlayerDied;

        GameSpeed.Reset();

        if (autoStartOnLoad)
        {
            autoStartOnLoad = false;
            StartGame();
        }
        else
        {
            EnterMainMenu();
        }
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePressed;
        pauseAction.Disable();
        PlayerHealth.Died -= OnPlayerDied;
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (State == GameState.Playing)
        {
            EnterPaused();
        }
        else if (State == GameState.Paused)
        {
            ResumeGame();
        }
    }

    private void OnPlayerDied()
    {
        EnterGameOver();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        State = GameState.Playing;

        mainMenuPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        hud.SetActive(true);

        player.SetActive(true);
        obstacleSpawner.SetActive(true);
        scoreTicker.SetActive(true);
    }

    public void ResumeGame()
    {
        State = GameState.Playing;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RetryGame()
    {
        autoStartOnLoad = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        autoStartOnLoad = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private void EnterMainMenu()
    {
        State = GameState.MainMenu;

        mainMenuPanel.SetActive(true);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        hud.SetActive(false);

        player.SetActive(false);
        obstacleSpawner.SetActive(false);
        scoreTicker.SetActive(false);
    }

    private void EnterPaused()
    {
        State = GameState.Paused;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void EnterGameOver()
    {
        State = GameState.GameOver;
        Time.timeScale = 0f;
        finalScoreText.text = $"Score: {ScoreManager.CurrentScore}";
        gameOverPanel.SetActive(true);
    }
}

using UnityEngine;
using TMPro;

public enum GameState
{
    MainMenu,
    LevelSelect,
    Playing,
    Paused,
    LevelCompleted,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState;

    public GameObject mainMenuUI;
    public GameObject levelSelectUI;
    public GameObject gameplayUI;
    public GameObject gameoverUI;

    public TextMeshProUGUI distanceText;

    public float score;

     void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetState(GameState.MainMenu);
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            distanceText.text = "Total distance: " + ((int) score).ToString()  + "m";
        }
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        mainMenuUI.GetComponent<Canvas>().enabled = false;
        // levelSelectUI.SetActive(false);
        gameplayUI.GetComponent<Canvas>().enabled = false;
        gameoverUI.GetComponent<Canvas>().enabled = false;

        switch (newState)
        {
            case GameState.MainMenu:
                mainMenuUI.GetComponent<Canvas>().enabled = true;
                break;

            case GameState.LevelSelect:
                // levelSelectUI.SetActive(true);
                break;

            case GameState.Playing:
                gameplayUI.GetComponent<Canvas>().enabled = true;
                break;

            case GameState.GameOver:
                gameoverUI.GetComponent<Canvas>().enabled = true;
                break;
        }
    }

    public void OnPlayGame()
    {
        SetState(GameState.Playing);
    }

    public void OnGameOver()
    {
        SetState(GameState.GameOver);
    }

    public void OnGoHome()
    {
        // SceneManager.LoadScene(0);  // Reset the whole scene
        // SetState(GameState.MainMenu);
    }
}

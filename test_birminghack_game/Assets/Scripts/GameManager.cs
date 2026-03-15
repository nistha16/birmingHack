using UnityEngine;

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


     void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetState(GameState.MainMenu);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;

        mainMenuUI.GetComponent<Canvas>().enabled = false;
        // levelSelectUI.SetActive(false);
        gameplayUI.GetComponent<Canvas>().enabled = false;

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
        }
    }

    public void OnPlayGame()
    {
        SetState(GameState.Playing);
    }
}

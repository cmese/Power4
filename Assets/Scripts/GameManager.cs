using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public GameState prevState;
    public static event Action<GameState> OnGameStateChanged;

    [SerializeField] private PlayerChip playerChipPrefab;
    private Vector3 startPos = new Vector3(3.5f, -6.0f, -7.0f); // 7/2, -(7-1), -(14/2)
    private Quaternion startRot = Quaternion.Euler(new Vector3(-90, 0, 0));

    [SerializeField] private Gameboard gameboardPrefab;
    private Gameboard gameboard;
    private Vector3 gameboardPos = new Vector3(3.5f, -3.0f, 0);

    private List<GameObject> playerWinList;
    private List<GameObject> enemyWinList;

    [SerializeField] private Color playerChipColor; //choosing in editor for now
    //TODO: let user choose color and save/load locally

    void Awake() {
        Instance = this;
        //TODO: set playerChipColor from local
    }

    void Start() {
        //TODO: fit gameboard into viewport before instantiating
        gameboard = Instantiate(gameboardPrefab, gameboardPos, Quaternion.identity);
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState) {
        prevState = State;
        State = newState;

        switch (newState) {
            case GameState.MainMenu:
                HandleMainMenu();
                break;
            case GameState.OnlineMenu:
                HandleOnlineMenu();
                break;
            case GameState.ModeMenu:
                HandleModeMenu();
                break;
            case GameState.SettingsMenu:
                break;
            case GameState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case GameState.EnemyTurn:
                break;
            case GameState.Decide:
                HandleDecide();
                break;
            case GameState.Victory:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleMainMenu() {
    }

    private void HandleOnlineMenu() {
        //calling menumanager for signin popups, matchmaking popups, friend invite popups, etc
    }

    private void HandleModeMenu() {
    }

    private void HandlePlayerTurn() {
        //instantiate player chip
        var playerChip = Instantiate(playerChipPrefab, startPos, startRot);
        playerChip.Init(playerChipColor);
    }

    private void HandleDecide() {
        StartCoroutine(gameboard.CheckForWins());
       //horizontal check
       //vertical check
       //Diaganol check


        //check for win/loss
        //if win UpdateGameState(GameState.Victory)
        //else UpdateGameState(GameState.Lose)
    }

    public Gameboard GetGameboard() {
        return gameboard;
    }
    //TODO: change this to get from local save
    public Color GetPlayerChipColor() {
        return playerChipColor;
    }

}

public enum GameState {
    MainMenu,
    OnlineMenu,
    ModeMenu,
    SettingsMenu,
    PlayerTurn,
    EnemyTurn,
    Decide,
    Victory,
    Lose
}



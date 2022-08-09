/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerOld : MonoBehaviour
{
    
    public static GameManagerOld Instance;
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

    private bool menu = false;
    private bool online = false;
    private int lastPlayerMove;
    private GameState currentMenu;

    [SerializeField] private Color playerChipColor; //choosing in editor for now
    //TODO: let user choose color and save/load locally

    void Awake() {
        Instance = this;
        gameboard = Instantiate(gameboardPrefab, gameboardPos, Quaternion.identity);
        //TODO: set playerChipColor from local
    }

    void Start() {
        //TODO: fit gameboard into viewport before instantiating
        //gameboard = Instantiate(gameboardPrefab, gameboardPos, Quaternion.identity);
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
        menu = true;
        online = false;
    }

    private void HandleOnlineMenu() {
        online = true;
        //calling menumanager for signin popups, matchmaking popups, friend invite popups, etc
    }

    private void HandleModeMenu() {
        menu = false;
    }

    private void HandlePlayerTurn() {
        //instantiate player chip
        var playerChip = Instantiate(playerChipPrefab, startPos, startRot);
        playerChip.Init(playerChipColor);
    }

    private void HandleDecide() {
        StartCoroutine(gameboard.Decide(menu));

        MenuManager.Instance.UpdatePanel(-1);
        //check for win/loss depending on gamemode 
        //if win UpdateGameState(GameState.Victory)
        //else UpdateGameState(GameState.Lose)
    }

    //chip was successfully inserted into gameboard
    public void ChipInserted(int col) {
        if (menu) { //if still in one of the menu states
            MenuManager.Instance.getMenu
        }
        if (MenuManager.Instance.getMenu().TryGetValue(prevState, out var menuDict)) {
            if (menuDict.TryGetValue(col, out var menuItem)) {
                GameManager.Instance.UpdateGameState(menuItem.getNextState());
            }
        }
        GameManager.Instance.UpdateGameState(GameState.Decide);
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
}*/



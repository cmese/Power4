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

    [SerializeField] private Gameboard currentBoard;

    void Awake() {
        Instance = this;
    }

    void Start() {
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
        //subscribe to unusued menu event - empty the board, reset panel text, reset preview chips
        currentBoard.ClearBoard();
    }

    private void HandleOnlineMenu() {
        //calling menumanager for signin popups, matchmaking popups, friend invite popups, etc
        currentBoard.ClearBoard();
    }

    private void HandleModeMenu() {
        currentBoard.ClearBoard();
    }

    private void HandleDecide() {
       //horizontal check
       //vertical check
       //Diaganol check



        //check for win/loss
        //if win UpdateGameState(GameState.Victory)
        //else UpdateGameState(GameState.Lose)
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



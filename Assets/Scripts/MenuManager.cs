using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Gameboard currentBoard;

    [SerializeField] private TMPro.TextMeshProUGUI infoText;

    public static MenuManager Instance;
    //public MenuState State;
    //public static event Action<MenuState> OnMenuStateChanged;

    private Dictionary<int, string[]> mainMenuDict = new Dictionary<int, string[]>() {
        { 1, new string[2]{"MULTI", "Play Online"} },
        { 3, new string[2]{ "COMP", "Play AI" } },
        { 5, new string[2]{ "HIGH", "View Leader Board"} }
    };
    private Dictionary<int, string[]> onlineMenuDict = new Dictionary<int, string[]>() {
        { 0, new string[2]{"<<<<<", "<---Back"} },
        { 2, new string[2]{ "MATCH", "Find a Match" } },
        { 4, new string[2]{ "DUEL", "Duel a Friend"} }
    };
    private Dictionary<int, string[]> modeMenuDict = new Dictionary<int, string[]>() {
        { 0, new string[2]{"<<<<<", "<---Back"} },
        { 1, new string[2]{ "4OUR", "Original Connect 4" } },
        { 3, new string[2]{ "POW4", "Original With PowerUps"} },
        { 5, new string[2]{ "BLITZ", "Power4 Blitz"} }
    };

    //remove this, use strings, instantiate them and insert them  1 by 1
    private char[,] mainMenu = new char[6,7] {
        { '.', '.', '.', '.', '.', '.', '.' },
        { '.', 'M', '.', 'ﲾ', '.', '', '.' },
        { '.', 'U', '.', 'C', '.', 'H', '.' },
        { '.', 'L', '.', 'O', '.', 'I', '.' },
        { '.', 'T', '.', 'M', '.', 'G', '.' },
        { '.', 'I', '.', 'P', '.', 'H', '.' },
    };
    private char[,] onlineMenu = new char[6,7] {
        { '.', '.', '.', '.', '.', '.', '.' },
        { '<', '.', 'M', '.', '.', '.', '.' },
        { '<', '.', 'A', '.', 'D', '.', '.' },
        { '<', '.', 'T', '.', 'U', '.', '.' },
        { '<', '.', 'C', '.', 'E', '.', '.' },
        { '<', '.', 'H', '.', 'L', '.', '.' },
    };
    private char[,] modeMenu = new char[6,7] {
        { '.', '.', '.', '.', '.', '.', '.' },
        { '<', '.', '.', '.', '.', 'B', '.' },
        { '<', '4', '.', 'P', '.', 'L', '.' },
        { '<', 'O', '.', 'O', '.', 'I', '.' },
        { '<', 'U', '.', 'W', '.', 'T', '.' },
        { '<', 'R', '.', '4', '.', 'Z', '.' },
    };

    void Awake() {
        Instance = this;
        GameManager.OnGameStateChanged += GameStateChanged;
    }

    void OnDestroy() {
        GameManager.OnGameStateChanged -= GameStateChanged;
    }

    private void GameStateChanged(GameState state) {
        switch (state) {
            case GameState.MainMenu:
                currentBoard.DictToBoard(mainMenuDict);
                //currentBoard.DictToBoard(menu[state]);
                break;
            case GameState.OnlineMenu:
                currentBoard.DictToBoard(onlineMenuDict);
                break;
            case GameState.ModeMenu:
                currentBoard.DictToBoard(modeMenuDict);
                break;
            default:
                break;
                //throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void UpdatePanel(int col) {
        infoText.gameObject.SetActive(false);
        switch (GameManager.Instance.State) {
            case GameState.MainMenu:
                UpdatePanelText(mainMenuDict, col);
                break;
            case GameState.OnlineMenu:
                UpdatePanelText(onlineMenuDict, col);
                break;
            case GameState.ModeMenu:
                UpdatePanelText(modeMenuDict, col);
                break;
            default:
                //infoText.gameObject.SetActive(false);
                break;
                //throw new ArgumentOutOfRangeException(nameof(GameManager.Instance.State), GameManager.Instance.State, null);
        }
    }

    private void UpdatePanelText(Dictionary<int, string[]> menuDict, int col) {
        if (menuDict.TryGetValue(col, out var details)) {
            infoText.text = details[1];
            infoText.gameObject.SetActive(true);
        }
    }
}
    /*
    public void UpdateMenuState(MenuState newState) {
        currentBoard.ClearBoard();
        State = newState;
        //select the correct array and dictionary
        switch (newState) {
            case GameState.Home:
                currentBoard.CreateMenuBoard(homeMenu);
                break;
            case MenuState.Online:
                currentBoard.CreateMenuBoard(onlineMenu);
                break;
            case MenuState.Mode:
                currentBoard.CreateMenuBoard(modeMenu);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        //OnMenuStateChanged?.Invoke(MenuState.Home); //Unused event
    }*/


/*
public enum MenuState {
    Home,
    Online,
    Mode
}*/




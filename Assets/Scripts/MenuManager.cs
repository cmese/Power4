using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
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

    void Awake() {
        Instance = this;
        GameManager.OnGameStateChanged += GameStateChanged;
    }

    void OnDestroy() {
        GameManager.OnGameStateChanged -= GameStateChanged;
    }

    private void GameStateChanged(GameState state) {
        Gameboard currentBoard = GameManager.Instance.GetGameboard();
        switch (state) {
            case GameState.MainMenu:
                currentBoard.ClearBoard();
                currentBoard.DictToBoard(mainMenuDict);
                GameManager.Instance.UpdateGameState(GameState.PlayerTurn);
                //currentBoard.DictToBoard(menu[state]);
                break;
            case GameState.OnlineMenu:
                currentBoard.ClearBoard();
                currentBoard.DictToBoard(onlineMenuDict);
                break;
            case GameState.ModeMenu:
                currentBoard.ClearBoard();
                currentBoard.DictToBoard(modeMenuDict);
                break;
            default:
                break;
                //throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void UpdatePanel(int col) {
        infoText.gameObject.SetActive(false);
        switch (GameManager.Instance.prevState) {
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
        }
    }

    private void UpdatePanelText(Dictionary<int, string[]> menuDict, int col) {
        if (menuDict.TryGetValue(col, out var details)) {
            infoText.text = details[1];
            infoText.gameObject.SetActive(true);
        }
    }
}

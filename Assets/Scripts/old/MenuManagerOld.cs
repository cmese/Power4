/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManagerOld : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI infoText;

    [SerializeField] private MenuChip menuChipPrefab;
    [SerializeField] private Chip dummyChipPrefab;

    public static MenuManagerOld Instance;
    //public MenuState State;
    //public static event Action<MenuState> OnMenuStateChanged;
    //

    public class MenuItem {
        private string chipText;
        private string moreInfoText;
        private GameState nextState;

        public MenuItem(string _chipString, string _infoString, GameState _nextState) {
            chipText = _chipString;
            moreInfoText = _infoString;
            nextState = _nextState;
        }

        public string getChipText() {
            return this.chipText;
        }

        public string getInfoText() {
            return this.moreInfoText;
        }

        public GameState getNextState() {
            return this.nextState;
        }
    }

    //for loop in gameboard 0 <i < 7
    private Dictionary<int, MenuItem> mainMenuDict = new Dictionary<int, MenuItem>() {
        { 1, new MenuItem("MULTI", "Play Online", GameState.OnlineMenu) },
        { 3, new MenuItem("COMP", "Play AI", GameState.ModeMenu) },
        { 5, new MenuItem("HIGH","View Leader Board", GameState.ModeMenu) },
    };
    private Dictionary<int, MenuItem> onlineMenuDict = new Dictionary<int, MenuItem>() {
        { 0, new MenuItem("<<<<<", "<---Back", GameState.MainMenu) },
        { 2, new MenuItem("MATCH", "Find an Opponent (Ranked)", GameState.ModeMenu) },
        { 4, new MenuItem("DUEL", "Duel a Friend (Casual)", GameState.ModeMenu) },
    };
    private Dictionary<int, MenuItem> modeMenuDict = new Dictionary<int, MenuItem>() {
        { 0, new MenuItem("<<<<<", "<---Back", GameState.MainMenu) },
        { 1, new MenuItem("ORIG", "Original Connect 4", GameState.ModeMenu) },
        { 3, new MenuItem("POW4", "Original with Powerups", GameState.ModeMenu) },
        { 5, new MenuItem("Blitz", "PowerUp Blitz", GameState.ModeMenu) },
    };

    private Dictionary<GameState, Dictionary<int, MenuItem>> menu;

    void Awake() {
        Instance = this;
        menu = new Dictionary<GameState, Dictionary<int, MenuItem>>();
        menu[GameState.MainMenu] = mainMenuDict;
        menu[GameState.OnlineMenu] = onlineMenuDict;
        menu[GameState.ModeMenu] = modeMenuDict;
        GameManagerOld.OnGameStateChanged += GameStateChanged;
    }

    void OnDestroy() {
        GameManagerOld.OnGameStateChanged -= GameStateChanged;
    }

    private void GameStateChanged(GameState state) {
        if (menu.TryGetValue(state, out var menuDict)) {
            MenuDictToBoard(menuDict);
        }
    }

    public void UpdatePanel(int col, GameState state) {
        infoText.gameObject.SetActive(false);
        if (menu.TryGetValue(state, out var menuDict)) {
            UpdatePanelText(menuDict, col);
        }
    }

    private void UpdatePanelText(Dictionary<int, MenuItem> menuDict, int col) {
        if (menuDict.TryGetValue(col, out var menuItem)) {
            infoText.text = menuItem.getInfoText();
            infoText.gameObject.SetActive(true);
        }
    }

    //assumes column string lengths < 6
    private void MenuDictToBoard(Dictionary<int, MenuItem> menuDict) {
        Gameboard currentBoard = GameManager.Instance.GetGameboard();
        currentBoard.ClearBoard();
        for (int col = 0; col < 7; col++) {
            int rowIndex = 5;
            if (menuDict.TryGetValue(col, out var menuItem)) {
                for (int i = menuItem.getChipText().Length - 1; i >= 0; i--) {
                    var menuChip = Instantiate(menuChipPrefab, new Vector3(col+0.5f, -rowIndex-0.5f, -0.75f), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    menuChip.SetColor(GameManagerOld.Instance.GetPlayerChipColor());
                    menuChip.SetText(menuItem.getChipText()[i].ToString());
                    currentBoard.AddChip(new Vector2Int(rowIndex, col), menuChip);
                    rowIndex--;
                }
            } else {
                for (int row = rowIndex; row >= 0; row--) {
                    var dummyChip = Instantiate(dummyChipPrefab, new Vector3(col+0.5f, -row-0.5f, -0.75f), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    dummyChip.SetColor(Color.black);
                    currentBoard.AddChip(new Vector2Int(row, col), dummyChip);
                }
            }
        }
    }

    public Dictionary<GameState, Dictionary<int, MenuItem>> getMenu() {
        return menu;
    }
}*/

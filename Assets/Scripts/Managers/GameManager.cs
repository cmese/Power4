using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public StateMachine stateMachine; //{get; private set;}
    public Queue<IState> stateQueue; // {get; private set;}
    [SerializeField] private Gameboard gameboard;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private PlayerChipManager playerChipPrefab;

    private Vector3 startPos = new Vector3(3.5f, -6.0f, -7.0f); //7/2, -(7-1), -(14/2)
    private Quaternion startRot = Quaternion.Euler(new Vector3(-90, 0, 0));
    public Color playerColor {get; private set;}
    private ChipManager previewChip;

    public bool isOnline;// {get; set;}
    public GameMode? gameMode;// {get; set;}

    void Awake() {
        Instance = this;
        stateMachine = new StateMachine();
        stateQueue = new Queue<IState>();
        isOnline = false;
        gameMode = null;
        //TODO: get/set player color from saved local settings
        playerColor = playerChipPrefab.GetColor();
    }

    void Start() {
        stateMachine.ChangeState(new MenuState(this, Menu.main));
    }

    void Update() {
        stateMachine.Update();
    }

    public void CreateMenuBoard(Dictionary<int, string[]> menuDict) {
         gameboard.MenuDictToBoard(menuDict);
    }

    public PlayerChipManager CreatePlayerChip() {
        return Instantiate(playerChipPrefab, startPos, startRot);
    }

    public int AddToBoard(int col, ChipManager chip) {
        return gameboard.insert(col, chip);
    }

    public IEnumerator ProcessBoard() {
        yield return StartCoroutine(gameboard.CheckForWins());
        stateMachine.ChangeState(stateQueue.Dequeue());
    }

    public void StartGame() {
        //stateMachine.ChangeState(new GameState(this));
    }

    public void UpdatePreviewChip(int currentCol) {
        if (previewChip) previewChip.gameObject.SetActive(false);
        if (currentCol > -1) {
            previewChip = gameboard.previewChips[currentCol];
            previewChip.SetColor(playerColor);
            previewChip.gameObject.SetActive(true);
        }
    }

    public void UpdateMenuText(bool isActive, string infoText) {
        uiManager.infoText.text = infoText;
        uiManager.infoText.gameObject.SetActive(isActive);
    }
}

public enum GameMode {
    Original,
    Power4,
    Blitz
}

public enum Menu {
    main,
    online,
    mode
}

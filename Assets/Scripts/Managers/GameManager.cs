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
    private PlayerChipManager playerChip;
    private ChipManager enemyChip;

    private Vector3 startPos = new Vector3(3.5f, -6.0f, -7.0f); //7/2, -(7-1), -(14/2)
    private Quaternion startRot = Quaternion.Euler(new Vector3(-90, 0, 0));
    public Color playerColor {get; private set;}
    private ChipManager previewChip;

    [SerializeField] private ChipManager enemyChipPrefab;
    //private ChipManager enemyChip;
    public Color enemyColor {get; private set;}

    public bool isOnline;// {get; set;}
    public GameMode? gameMode;// {get; set;}

    [SerializeField] Animator playAgainPanelAnimator;
    [SerializeField] GameObject playAgainPanel;

    void Awake() {
        Instance = this;
        stateMachine = new StateMachine();
        stateQueue = new Queue<IState>();
        isOnline = false;
        gameMode = null;
        //TODO: get/set player color from saved local settings
        playerColor = playerChipPrefab.GetColor();
        enemyColor = GetEnemyColor();
    }

    void Start() {
        CreatePlayerChip();
        stateMachine.ChangeState(new MenuState(this, Menu.main, playerChip));
    }

    void Update() {
        stateMachine.Update();

    }

    private Color GetEnemyColor() {
        Color.RGBToHSV(playerColor, out float H, out float S, out float V);
        float negativeH = (H + 0.5f) % 1f;
        Color negativeColor = Color.HSVToRGB(negativeH, S, V);
        return negativeColor;
    }

    public void CreateMenuBoard(Dictionary<int, string[]> menuDict) {
         gameboard.MenuDictToBoard(menuDict);
    }

    public PlayerChipManager CreatePlayerChip() {
        if (playerChip == null || playerChip.stateMachine.currentState is InBoardState)
            playerChip = Instantiate(playerChipPrefab, startPos, startRot);
            playerChip.DisableChip();
        return playerChip;
    }

    public ChipManager CreateEnemyChip(int enemyCol) {
        Vector3 enemyStartPos = new Vector3(enemyCol + 0.5f, 1-0.5f, -0.75f);
        enemyChip = Instantiate(enemyChipPrefab, enemyStartPos, startRot);
        enemyChip.SetColor(enemyColor);
        return enemyChip;
    }

    public int AddToBoard(int col, ChipManager chip) {
        return gameboard.insert(col, chip);
    }

    public int AIChoose() {
        return 0;
    }

    public IEnumerator ProcessBoard() {
        yield return StartCoroutine(gameboard.CheckForWins());
        //enqueue win / loss / tie / or opposite players turn
        stateMachine.ChangeState(stateQueue.Dequeue());
    }

    public void StartGame(int playerTurn) {
       gameboard.ClearBoard();
       //initialize AI?
       //initialize timer?
       if (playerTurn == 1) {
           stateQueue.Enqueue(new PlayerTurnState(this, CreatePlayerChip()));
       } else {
            stateQueue.Enqueue(new EnemyTurnState(this, CreatePlayerChip(), AIChoose()));
       }
       stateMachine.ChangeState(stateQueue.Dequeue());
    }

    public void Decide(HashSet<Vector2Int> playerSet, HashSet<Vector2Int> enemySet) {
        if (stateMachine.prevState is not MenuState) {
            switch (gameMode) {
                case GameMode.Original:
                    //TODO: TieState
                    if (playerSet.Count >= 4) {
                        stateQueue.Clear();
                        stateQueue.Enqueue(new WinState(this));
                    } else if (enemySet.Count >= 4) {
                        stateQueue.Clear();
                        stateQueue.Enqueue(new LoseState(this));
                    }
                    break;
                case GameMode.Power4:
                    break;
                case GameMode.Blitz:
                    break;
            }
        }
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

    public void TriggerPlayAgainPanel() {
        playAgainPanel.SetActive(true);
        playAgainPanelAnimator.SetTrigger("Show");
    }

    public void PlayAgainYesButton() {
        StartCoroutine(PlayAgainYes());
    }

    public void PlayAgainExitButton() {
        StartCoroutine(PlayAgainExit());
    }

    private IEnumerator PlayAgainYes() {
        stateMachine.ChangeState(new GameState(this));
        yield return new WaitForSeconds(2.0f); //change this from 2 seconds => total animation time length
        playAgainPanel.SetActive(false);
        //stateQueue.Enqueue(new GameState(this));
    }

    private IEnumerator PlayAgainExit() {
        stateMachine.ChangeState(new MenuState(this, Menu.mode, CreatePlayerChip()));
        yield return new WaitForSeconds(2.0f);
        playAgainPanel.SetActive(false);
        //stateQueue.Enqueue(new MenuState(this, Menu.mode, CreatePlayerChip()));
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

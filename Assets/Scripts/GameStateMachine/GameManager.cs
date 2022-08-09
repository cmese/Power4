using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameStateMachine stateMachine {get; private set;}
    [SerializeField] private Gameboard gameboardPrefab;
    private Gameboard gameboard;
    private Vector3 gameboardPos = new Vector3(3.5f, -3.0f, 0);

    public bool isOnline {get; set;}
    public GameMode? gameMode {get; set;}

    void Awake() {
        Instance = this;
        stateMachine = new GameStateMachine(); //master state machine
        gameboard = Instantiate(gameboardPrefab, gameboardPos, Quaternion.identity);
        isOnline = false;
        gameMode = null;
    }

    void Start() {
        stateMachine.ChangeState(new GameMenuState(this));
    }

    public Gameboard GetGameboard() {
        return gameboard;
    }

    public void AddToBoard(int col) {
        gameboard.insert(col);
    }
}

public enum GameMode {
    Original,
    Power4,
    Blitz
}

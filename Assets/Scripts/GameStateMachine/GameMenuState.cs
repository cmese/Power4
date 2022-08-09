using UnityEngine;

public class GameMenuState : GameState {
    GameManager gameManager;

    public GameMenuState(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void Enter() {
        Debug.Log("entering Menu state"); //start

    }

    public void Execute() {
        Debug.Log("updating Drag state"); //update
    }

    public void Exit() {
        Debug.Log("exiting Drag state");
    }
}

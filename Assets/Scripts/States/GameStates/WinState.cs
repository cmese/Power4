using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : IState
{
    private readonly GameManager gameManager;

    public WinState(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void Enter() {
        gameManager.TriggerPlayAgainPanel();
        Debug.Log("Entering WIN State");
    }

    public void Execute() {
    }

    public void Exit() {
        Debug.Log("Exiting WIN State");
    }
}

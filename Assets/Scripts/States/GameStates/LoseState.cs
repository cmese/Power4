using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : IState
{
    private readonly GameManager gameManager;

    public LoseState(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void Enter() {
        gameManager.TriggerPlayAgainPanel();
        Debug.Log("Entering Lose State");
    }

    public void Execute() {
    }

    public void Exit() {
        Debug.Log("Exiting Lose State");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessBoardState : IState
{
    private readonly GameManager gameManager;

    public ProcessBoardState(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void Enter() {
        Debug.Log("entering ProcessBoardState"); //start
        gameManager.StartCoroutine(gameManager.ProcessBoard());
    }

    public void Execute() {
    }

    public void Exit() {
        Debug.Log("Exiting ProcessBoardState");
    }
}

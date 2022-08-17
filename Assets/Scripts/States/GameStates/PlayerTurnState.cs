using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : IState
{
    private readonly GameManager gameManager;
    private PlayerChipManager playerChip;

    public PlayerTurnState(GameManager gameManager, PlayerChipManager playerChip) {
        this.gameManager = gameManager;
        this.playerChip = playerChip;
    }

    public void Enter() {
        Debug.Log("Entering PlayerTurnState");
        playerChip.EnableChip();
    }

    public void Execute() {
        if (playerChip.stateMachine.currentState is InBoardState) {
            gameManager.stateQueue.Enqueue(new ProcessBoardState(gameManager));
            gameManager.stateQueue.Enqueue(new EnemyTurnState(gameManager, gameManager.CreatePlayerChip(), gameManager.AIChoose())); //this will get cleared out if there is a win / loss
            gameManager.stateMachine.ChangeState(gameManager.stateQueue.Dequeue());
            //gameManager.stateMachine.ChangeState(new ProcessBoardState(gameManager));
            return;
        }
    }

    public void Exit() {
        Debug.Log("Exiting PlayerTurnState");
    }
}

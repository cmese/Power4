using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : IState
{
    private readonly GameManager gameManager;
    private PlayerChipManager playerChip;
    private ChipManager enemyChip;
    private int enemyCol;

    public EnemyTurnState(GameManager gameManager, PlayerChipManager playerChip, int enemyCol) {
        this.gameManager = gameManager;
        this.playerChip = playerChip;
        this.enemyCol = enemyCol;
        //this.enemyChip = gameManager.CreateEnemyChip(enemyCol);
    }

    public void Enter() {
        Debug.Log("Entering EnemyTurnState");
        playerChip.DisableChip();
        enemyChip = gameManager.CreateEnemyChip(enemyCol);
        int enemyRow = gameManager.AddToBoard(enemyCol, enemyChip);
        enemyChip.stateQueue.Enqueue(new MoveState(enemyChip, new Vector3(enemyCol+0.5f, -enemyRow-0.5f, -0.75f), 1.0f)); //move down board
        enemyChip.stateQueue.Enqueue(new InBoardState(enemyChip));
        enemyChip.stateMachine.ChangeState(enemyChip.stateQueue.Dequeue());
    }

    public void Execute() {
        if (enemyChip.stateMachine.currentState is InBoardState) {
            gameManager.stateQueue.Enqueue(new ProcessBoardState(gameManager));
            gameManager.stateQueue.Enqueue(new PlayerTurnState(gameManager, gameManager.CreatePlayerChip())); //this will get cleared out if there is a win / loss
            gameManager.stateMachine.ChangeState(gameManager.stateQueue.Dequeue());
            //gameManager.stateMachine.ChangeState(new ProcessBoardState(gameManager));
            return;
        }
    }

    public void Exit() {
        Debug.Log("Exiting EnemyTurnState");
    }
}

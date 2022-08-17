using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : IState
{
    private readonly GameManager gameManager;

    public GameState(GameManager gameManager) {
        this.gameManager = gameManager;
    }

    public void Enter() {
        Debug.Log("Entering GameState");
        //initialize game with game mode, players, etc
        //choose a player at random (for now) to start first
        gameManager.StartGame(Random.Range(0,2));
    }

    public void Execute() {
    }

    public void Exit() {
        Debug.Log("Exiting GameState");
    }
}

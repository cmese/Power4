using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuState : IState
{
    private readonly GameManager gameManager;
    private PlayerChipManager playerChip;
    private Menu menu;
    private Dictionary<int, string[]> mainMenuDict = new Dictionary<int, string[]>() {
        { 1, new string[] { "MULTI", "Play Online" } },
        { 3, new string[] { "COMP", "Play AI" } },
        { 5, new string[] { "HIGH","View Leader Board" } },
    };
    private Dictionary<int, string[]> modeMenuDict = new Dictionary<int, string[]>() {
        { 0, new string[] { "<<<<<", "<---Back" } },
        { 1, new string[] { "ORIG", "Original Connect 4"} },
        { 3, new string[] { "POW4", "Original with Powerups"} },
        { 5, new string[] { "Blitz", "PowerUp Blitz"} },
    };
    private Dictionary<int, string[]> onlineMenuDict = new Dictionary<int, string[]>() {
        { 0, new string[] { "<<<<<", "<---Back"} },
        { 2, new string[] { "MATCH", "Find an Opponent (Ranked)"} },
        { 4, new string[] { "DUEL", "Duel a Friend (Casual)"} },
    };

    public MenuState(GameManager gameManager, Menu menu)  {
        this.gameManager = gameManager;
        this.menu = menu;
    }

    public void Enter() {
        switch(menu) {
            case Menu.main:
                gameManager.CreateMenuBoard(mainMenuDict);
                break;
            case Menu.mode:
                gameManager.CreateMenuBoard(modeMenuDict);
                break;
            case Menu.online:
                gameManager.CreateMenuBoard(onlineMenuDict);
                break;
            default:
                Debug.Log("Error: no such Menu");
                break;
        }
        playerChip = gameManager.CreatePlayerChip();
    }

    public void Execute() {
        if (playerChip.stateMachine.currentState is InBoardState) {
            Debug.Log(gameManager);
            Debug.Log(gameManager.stateQueue);
            gameManager.stateQueue.Enqueue(new ProcessBoardState(gameManager));
            switch(menu) {
                case Menu.main:
                    HandleMainMenu();
                    break;
                case Menu.mode:
                    HandleModeMenu();
                    break;
                case Menu.online:
                    HandleOnlineMenu();
                    break;
                default:
                    Debug.Log("Error: no such Menu");
                    break;
            }
            gameManager.stateMachine.ChangeState(gameManager.stateQueue.Dequeue());
            return;
        }
    }

    public void Exit() {
        Debug.Log("Exiting Menu State");
    }

    private void HandleMainMenu() {
        if (playerChip.currentCol == 1) gameManager.isOnline = true;
        gameManager.stateQueue.Enqueue(new MenuState(gameManager, Menu.mode));
    }

    private void HandleModeMenu() {
        switch(playerChip.currentCol) {
            case 0:
                gameManager.isOnline = false;
                gameManager.stateQueue.Enqueue(new MenuState(gameManager, Menu.main));
                return;
            case 1:
                gameManager.gameMode = GameMode.Original;
                break;
            case 3:
                gameManager.gameMode = GameMode.Power4;
                break;
            case 5:
                gameManager.gameMode = GameMode.Blitz;
                break;
            default:
                Debug.Log("Error: Invalid menu column");
                break;
        }
        if (gameManager.isOnline) {
            gameManager.stateQueue.Enqueue(new MenuState(gameManager, Menu.online));
        } else {
            gameManager.StartGame();
        }
    }

    private void HandleOnlineMenu() {
        if (playerChip.currentCol == 0) {
            gameManager.gameMode = null;
            gameManager.stateQueue.Enqueue(new MenuState(gameManager, Menu.mode));
        }
    }
}

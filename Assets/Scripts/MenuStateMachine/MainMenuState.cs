using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : MenuState {
    MenuManager menuManager;
    private Dictionary<int, string[]> mainMenuDict = new Dictionary<int, string[]>() {
        { 1, new string[] { "MULTI", "Play Online" } },
        { 3, new string[] { "COMP", "Play AI" } },
        { 5, new string[] { "HIGH","View Leader Board" } },
    };

    public MainMenuState(MenuManager menuManager) {
        this.menuManager = menuManager;
    }

    public void Enter() {
        Debug.Log("Entering Main Menu State");
        menuManager.MenuDictToBoard(mainMenuDict);
    }

    public void Exit() {
        Debug.Log("Exiting Main Menu State");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeMenuState : MenuState {
    MenuManager menuManager;
    private Dictionary<int, string[]> modeMenuDict = new Dictionary<int, string[]>() {
        { 0, new string[] { "<<<<<", "<---Back" } },
        { 1, new string[] { "ORIG", "Original Connect 4"} },
        { 3, new string[] { "POW4", "Original with Powerups"} },
        { 5, new string[] { "Blitz", "PowerUp Blitz"} },
    };
    public ModeMenuState(MenuManager menuManager) {
        this.menuManager = menuManager;
    }

    public void Enter() {
        Debug.Log("Entering main menu state");
        menuManager.MenuDictToBoard(modeMenuDict);
    }

    public void Exit() {
        Debug.Log("exiting Drag state");
    }
}

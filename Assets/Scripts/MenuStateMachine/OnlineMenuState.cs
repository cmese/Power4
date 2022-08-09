using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMenuState : MenuState {
    MenuManager menuManager;
    private Dictionary<int, string[]> onlineMenuDict = new Dictionary<int, string[]>() {
        { 0, new string[] { "<<<<<", "<---Back"} },
        { 2, new string[] { "MATCH", "Find an Opponent (Ranked)"} },
        { 4, new string[] { "DUEL", "Duel a Friend (Casual)"} },
    };
    public OnlineMenuState(MenuManager menuManager) {
        this.menuManager = menuManager;
    }

    public void Enter() {
        Debug.Log("Entering main menu state");
    }

    public void Exit() {
        Debug.Log("exiting Drag state");
    }
}

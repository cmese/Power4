using UnityEngine;

public class DragState : IState {
    private readonly PlayerChipManager playerManager;

    public DragState(PlayerChipManager playerManager) {
        this.playerManager = playerManager;
    }

    public void Enter() {
        Debug.Log("entering Drag state"); //start
        //this.owner.StartCoroutine(StartLerp(finalLerpPos, duration));
    }

    public void Execute() {
    }

    public void Exit() {
        Debug.Log("exiting Drag state");
    }
}
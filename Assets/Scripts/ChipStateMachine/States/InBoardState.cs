using UnityEngine;

public class InBoardState : ChipState {
    private readonly PlayerChipManager playerManager;

    public InBoardState(PlayerChipManager playerManager) {
        this.playerManager = playerManager;
    }

    public void Enter() {
        Debug.Log("entering Drag state"); //start
        //this.owner.StartCoroutine(StartLerp(finalLerpPos, duration));
    }

    public void Execute() {
        Debug.Log("updating Drag state"); //update
    }

    public void Exit() {
        Debug.Log("exiting Drag state");
    }
}

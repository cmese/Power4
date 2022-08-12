using UnityEngine;

public class DragState : IState {
    private readonly PlayerChipManager playerManager;
    private int currentCol = -1;

    public DragState(PlayerChipManager playerManager) {
        this.playerManager = playerManager;
    }

    public void Enter() {
        Debug.Log("entering Drag state"); //start
        //this.owner.StartCoroutine(StartLerp(finalLerpPos, duration));
    }

    public void Execute() {
        if (currentCol != playerManager.currentCol) {
            GameManager.Instance.UpdatePreviewChip(playerManager.currentCol);
            currentCol = playerManager.currentCol;
        }
    }

    public void Exit() {
        Debug.Log("exiting Drag state");
    }
}

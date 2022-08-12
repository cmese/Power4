using UnityEngine;

public class InBoardState : IState {
    private readonly ChipManager chipManager;

    public InBoardState(ChipManager chipManager) {
        this.chipManager = chipManager;
    }

    public void Enter() {
        Debug.Log("Entered InBoardState");
    }

    public void Execute() {
    }

    public void Exit() {
        Debug.Log("Exiting InBoardState");
    }
}

using UnityEngine;

//TODO: bounce / glow animation
public class IdleState : ChipState {
    private readonly PlayerChipManager playerManager;

    public IdleState(PlayerChipManager playerManager) {
        this.playerManager = playerManager;
    }

    public void Enter() {
        Debug.Log("entering Idle state"); //start
    }

    public void Execute() {
        Debug.Log("updating Idle state"); //update
    }

    public void Exit() {
        //stop glow / bounce - set back to defaults
        Debug.Log("exiting Idle state");
    }
}

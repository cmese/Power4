using UnityEngine;

//TODO: bounce / glow animation
public class IdleState : IState {
    private readonly PlayerChipManager playerManager;

    public IdleState(PlayerChipManager playerManager) {
        this.playerManager = playerManager;
    }

    public void Enter() {
        Debug.Log("entering Idle state"); //start
    }

    public void Execute() {
    }

    public void Exit() {
        //stop glow / bounce - set back to defaults
        Debug.Log("exiting Idle state");
    }
}

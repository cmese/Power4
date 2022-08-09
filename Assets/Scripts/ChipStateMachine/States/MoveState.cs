using System.Collections;
using UnityEngine;

public class MoveState : ChipState {
    private readonly PlayerChipManager playerManager;
    public Vector3 finalLerpPos;
    float duration;

    public MoveState(PlayerChipManager playerManager, Vector3 finalLerpPos, float duration) {
        this.playerManager = playerManager;
        this.finalLerpPos = finalLerpPos;
        this.duration = duration;
    }

    public void Enter() {
        Debug.Log("entering Move state"); //start
        playerManager.StartCoroutine(LerpPosition(finalLerpPos, duration));
    }

    public void Execute() {
        Debug.Log("updating Move state"); //update
    }

    public void Exit() {
        Debug.Log("exiting Move state");
    }

    IEnumerator LerpPosition(Vector3 finalLerpPos, float duration) {
        float time = 0;
        Vector3 startPosition = playerManager.transform.position;
        while (time < duration) {
            playerManager.transform.position = Vector3.Lerp(startPosition, finalLerpPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        playerManager.transform.position = finalLerpPos;
        playerManager.stateMachine.ChangeState(playerManager.stateQueue.Dequeue());
    }
}

using System.Collections;
using UnityEngine;

public class MoveState : IState {
    private readonly ChipManager chipManager;
    public Vector3 finalLerpPos;
    float duration;

    public MoveState(ChipManager chipManager, Vector3 finalLerpPos, float duration) {
        this.chipManager = chipManager;
        this.finalLerpPos = finalLerpPos;
        this.duration = duration;
    }

    public void Enter() {
        Debug.Log("entering Move state"); //start
        chipManager.StartCoroutine(LerpPosition(finalLerpPos, duration));
    }

    public void Execute() {
    }

    public void Exit() {
        Debug.Log("exiting Move state");
    }

    IEnumerator LerpPosition(Vector3 finalLerpPos, float duration) {
        float time = 0;
        Vector3 startPosition = chipManager.transform.position;
        while (time < duration) {
            chipManager.transform.position = Vector3.Lerp(startPosition, finalLerpPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        chipManager.transform.position = finalLerpPos;
        chipManager.stateMachine.ChangeState(chipManager.stateQueue.Dequeue());
    }
}

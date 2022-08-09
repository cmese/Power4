public interface ChipState {
    public void Enter();
    public void Execute();
    public void Exit();
}

public class ChipStateMachine {
    public ChipState currentState; //{get; private set;}

    public void ChangeState(ChipState newState) {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public void Update() {
        if (currentState != null) currentState.Execute();
    }
}

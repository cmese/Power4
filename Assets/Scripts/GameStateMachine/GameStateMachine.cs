public interface GameState {
    public void Enter();
    public void Execute();
    public void Exit();
}

public class GameStateMachine {
    public GameState currentState; //{get; private set;}

    public void ChangeState(GameState newState) {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public void Update() {
        if (currentState != null) currentState.Execute();
    }
}

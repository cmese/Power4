public interface IState {
    public void Enter();
    public void Execute();
    public void Exit();
}

public class StateMachine {
    public IState currentState {get; private set;}
    public IState prevState {get; private set;}

    public void ChangeState(IState newState) {
        if (currentState != null)
            currentState.Exit();
        prevState = currentState;

        currentState = newState;
        currentState.Enter();
    }

    public void Update() {
        if (currentState != null) currentState.Execute();
    }
}

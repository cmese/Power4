public interface MenuState {
    public void Enter();
    public void Exit();
}

public class MenuStateMachine {
    public MenuState currentState; //{get; private set;}

    public void ChangeState(MenuState newState) {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }
}

using UnityEngine;

public class PlayerStateMachine 
{
    // getter for the reference value stored in Player State
    public PlayerState _currentState { get; private set; }

    public void StartState(PlayerState _startState) {
        _currentState = _startState;
        _currentState.OnEnter();
    }

    public void ChangeStateTo(PlayerState state) {
        //_currentState.OnExit();
        _currentState = state;
        state.OnEnter();
    }
}

using UnityEngine;

public class PlayerStateMachine 
{
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

    public PlayerStateList name()
    {
        return _currentState._name;
    }
}

using UnityEngine;

public class JumpState : PlayerState
{
    private float _maxJumpHeight;
    private float _jumpDuration;

    private float _minJumpVelocity;
    private float _maxJumpVelocity;
    public JumpState(Player player, PlayerStateMachine state, float maxHeight, float duartion) : base(player, state)
    { 
        _maxJumpHeight = maxHeight;
        _jumpDuration = duartion;

        player._gravity = -(2 * _maxJumpHeight) / Mathf.Pow(_jumpDuration, 2);
        _maxJumpVelocity = (2f * _maxJumpHeight) / _jumpDuration;
    }

    public override void OnEnter()
    {
        player._velocity.y = _maxJumpVelocity; // Apply jump once
    }
    public override void Update()
    {
        if (player._velocity.y <= 0f)
        {
            stateMachine.ChangeStateTo(player._fall_state);
        }
    }

    public override void OnExit()
    {
        stateMachine.ChangeStateTo(player._fall_state);
    }
}

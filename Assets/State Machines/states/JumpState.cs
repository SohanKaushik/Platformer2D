using UnityEngine;

public class JumpState : PlayerState
{
    private float _maxJumpHeight;
    private float _jumpDuration;

    private float _minJumpVelocity;
    private float _maxJumpVelocity;
    public JumpState(Player player, PlayerStateMachine state, float maxHeight, float duartion, float maxJumpVelocity)
        : base(player, state, PlayerStateList.Jumping)
    { 
        _maxJumpHeight = maxHeight;
        _jumpDuration = duartion;
        _maxJumpVelocity = maxJumpVelocity;
    }

    public override void OnEnter()
    {
        // # apply jump once
        player._velocity.y = _maxJumpVelocity;
    }

    public override void Update()
    {
       if (player._velocity.y >= _maxJumpHeight)
       {
            stateMachine.ChangeStateTo(player._idle_state); 
       }
    }
}

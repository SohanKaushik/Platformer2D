using System.Collections;
using UnityEngine;

public class JumpState : PlayerState
{
    private float _maxJumpHeight;
    private float _jumpDuration;
    private float _maxJumpVelocity;

    public JumpState(yuo player, PlayerStateMachine state, float maxHeight, float duration, float maxJumpVelocity)
        : base(player, state, PlayerStateList.Jumping)
    {
        _maxJumpHeight = maxHeight;
        _jumpDuration = duration;
        _maxJumpVelocity = maxJumpVelocity;
    }

    public override void OnEnter()
    {
        // Apply jump velocity
        player._velocity.y = _maxJumpVelocity;
    }

    public override void Update()
    {

        // # fall
        if (player._velocity.y <= _maxJumpVelocity) {
            stateMachine.ChangeStateTo(player._fall_state);
            player.jumpRequest = false;
        }
    }
}

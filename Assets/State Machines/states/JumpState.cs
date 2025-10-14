using System.Data.Common;
using UnityEngine;

public class JumpState : PlayerState
{
    private float _maxJumpHeight;
    private float _jumpDuration;
    private float _maxJumpVelocity;

    public JumpState(Player player, PlayerStateMachine state, float maxHeight, float duration, float maxJumpVelocity)
        : base(player, state, PlayerStateList.Jumping)
    {
        _maxJumpHeight = maxHeight;
        _jumpDuration = duration;
        _maxJumpVelocity = maxJumpVelocity;
    }

    public override void OnEnter()
    {
        player._velocity.y = _maxJumpVelocity;
        player._velocity += player.LiftBoost;

        player._isDashing = false;

        player.currentPlatform = null;
    }

    public override void Update()
    {
        if (player.IsWallClimbAllowed())
        {
            stateMachine.ChangeStateTo(player._wall_climb_state);
            return;
        }
        else stateMachine.ChangeStateTo(player._fall_state);
    }
}

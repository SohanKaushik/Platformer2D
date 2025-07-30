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
        Debug.Log(player.jumpBufferCounter);
        player._velocity.y = _maxJumpVelocity;
    }
}

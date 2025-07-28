using UnityEngine;

public class FallState : PlayerState
{
    private float _terminalMultiplier;
    public FallState(Player player, PlayerStateMachine state, float terminalMultiplier) : base(player, state, PlayerStateList.Falling)
    {
        _terminalMultiplier = terminalMultiplier;
    }

    public override void Update()
    {
        if (player.isGrounded()) {
            stateMachine.ChangeStateTo(player._idle_state);
            return;
        }

        // # jump 
        if (player.jumpRequest && player._coyoteTimer >= 0.0f)
        {
            stateMachine.ChangeStateTo(player._jump_state);
            return;
        }
    }

    public override void FixedUpdate()
    {
        // # it has a depecdency to the jump Height and duration
        player._velocity.x = player.GetAxisDirections().x * player._footSpeed;

        var fallSpeed = player._velocity.y + player._gravity * Time.fixedDeltaTime;
        player._velocity.y = Mathf.Max(fallSpeed, -_terminalMultiplier);

    }
}

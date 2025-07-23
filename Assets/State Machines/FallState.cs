using UnityEngine;

public class FallState : PlayerState
{
    public FallState(Player player, PlayerStateMachine state) : base(player, state)
    {

    }

    public override void Update()
    {
        if (player._controller._colldata.below) {
            stateMachine.ChangeStateTo(player._idle_state);
        }
    }
    public override void FixedUpdate()
    {
        player._velocity.x = player.GetAxisDirections().x * player._footSpeed;
        player._velocity.y += player._gravity * Time.fixedDeltaTime;
    }
}

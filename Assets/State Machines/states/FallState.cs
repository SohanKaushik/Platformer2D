using UnityEngine;

public class FallState : PlayerState
{
    public FallState(yuo player, PlayerStateMachine state) : base(player, state, PlayerStateList.Falling)
    { }

    public override void FixedUpdate()
    {

        if(player.isGrounded()) { 
            stateMachine.ChangeStateTo(player._idle_state);
            return;
        }

        // # it has a depecdency to the jump Height and duration
        player._velocity.x = player.GetAxisDirections().x * player._footSpeed;
        player._velocity.y += player._gravity * Time.fixedDeltaTime;
    }
}

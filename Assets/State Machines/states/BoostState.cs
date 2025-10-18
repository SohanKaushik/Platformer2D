using UnityEngine;

public class BoostState : PlayerState
{
    public BoostState(Player player, PlayerStateMachine state, PlayerStateList name) : base(player, state, name)
    {
    }

    public override void OnEnter()
    {
        player._velocity += player.LiftBoost;
    }

    public override void Update()
    {
        if (player.IsCollided() || player.isGrounded()) {
            player._stateMachine.ChangeStateTo(player._idle_state);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        if(Mathf.Sign(player.LiftBoost.x) != player.GetFacings()){
            player._stateMachine.ChangeStateTo(player._fall_state);
        }

        player._velocity.y += player._gravity * Time.deltaTime;
    }
}

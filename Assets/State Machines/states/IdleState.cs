using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class IdleState : PlayerState {

    public IdleState(Player player, PlayerStateMachine state) : base(player, state, PlayerStateList.Idle) {
    
    }

    public override void OnEnter() {
        player._velocity.x = 0.0f;
        player._coyoteTimer = player._coyoteTime;
    }

    public override void FixedUpdate() {

        // # jump 
        if (player.jumpRequest) {
            stateMachine.ChangeStateTo(player._jump_state);
            return;
        }

        // # run
        if (Mathf.Abs(player.GetAxisDirections().x) > Mathf.Epsilon)
        {
            stateMachine.ChangeStateTo(player._run_state);
        }
    }
}

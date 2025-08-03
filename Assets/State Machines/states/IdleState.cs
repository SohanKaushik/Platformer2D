using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class IdleState : PlayerState {

    public IdleState(Player player, PlayerStateMachine state) : base(player, state, PlayerStateList.Idle) {
    
    }

    public override void OnEnter() {
        player._velocity.x = 0.0f;
        player._velocity.y = -0.001f; // applying minimal downward force 
        player._smooothfactorx = 0.0f;
    }


    public override void Update()
    {
        // # dash
        if (player._context.dashRequest) {
            stateMachine.ChangeStateTo(player._dash_state);
            return;
        }

        // # jump 
        if (player.jumpBufferCounter > 0.0f) {
            stateMachine.ChangeStateTo(player._jump_state);
           
            return;
        }
    }

    public override void FixedUpdate() {
        // # run
        if (Mathf.Abs(player.GetAxisDirections().x) > 0.1f) {
            stateMachine.ChangeStateTo(player._run_state);
            return;
        }
    }
}

using UnityEngine;

public class IdleState : PlayerState {

    public IdleState(Player player, PlayerStateMachine state) : base(player, state, PlayerStateList.Idle) {
    
    }

    public override void OnEnter() {

        player._velocity = Vector3.zero;
        player._smooothfactorx = 0.0f;
        
        player._isDashing = false;
        player._wallClimbTimeout = false;
    }


    public override void Update()
    {
        //// # dash
        //if (player._context.dashRequest) {
        //    stateMachine.ChangeStateTo(player._dash_state);
        //    return;
        //}

        // # jump && dash
        if (player.jumpBufferCounter > 0.0f) {
            stateMachine.ChangeStateTo(player._jump_state);
            return;
        }

        // # dash
        if (player.IsDashAllowed() || player.jumpBufferCounter >= 0.0f) {
            stateMachine.ChangeStateTo(player._dash_state);
            return;
        }

        // # wall climbing
        if (player.IsWallClimbAllowed()) {
            stateMachine.ChangeStateTo(player._wall_climb_state);
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

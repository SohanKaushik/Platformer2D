using UnityEngine;

public class IdleState : PlayerState {

    private Vector3 originialPlayerSize;
    public IdleState(Player player, PlayerStateMachine state) : base(player, state, PlayerStateList.Idle) {
        originialPlayerSize = player.transform.localScale;
    }

    public override void OnEnter() {

        player._velocity = Vector3.zero;
        player._smooothfactorx = 0.0f;
        
        player._isDashing = false;
        //player.SetIsGrounded(true);
        player._wallClimbTimeout = false;
    }


    public override void Update()
    {
        // # bruh
        if (player.GetAxisDirections().y < 0.1 && player.GetAxisDirections().y != 0f)
        {
            player.GetComponent<SpriteRenderer>().size = new Vector3(originialPlayerSize.x, originialPlayerSize.y * 0.5f, originialPlayerSize.z);
        }
        else player.GetComponent<SpriteRenderer>().size = originialPlayerSize;

        // # jump && dash
        if (player.jumpBufferCounter > 0.0f)
        {
            stateMachine.ChangeStateTo(player._jump_state);
            return;
        }

        // # dash
        if (player.IsDashAllowed()) {
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

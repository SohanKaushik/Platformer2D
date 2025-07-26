using UnityEngine;

public class IdleState : PlayerState {
    public IdleState(Player player, PlayerStateMachine state) : base(player, state, PlayerStateList.Idle)
    { }

    public override void OnEnter() {
    }

    public override void Update() {

        // # run
        if (Mathf.Abs(player.GetAxisDirections().x) > 0) {
            stateMachine.ChangeStateTo(player._run_state);
        }

        // # jump
        if (Input.GetKeyDown(KeyCode.Space) && player._controller.isGrounded())
        {
            stateMachine.ChangeStateTo(player._jump_state);
        }

        // # fall
        if(!player._controller.isGrounded()) {
            stateMachine.ChangeStateTo(player._fall_state);
        }
    }

    public override void FixedUpdate() {
        player._velocity.x = player.GetAxisDirections().x * player._footSpeed;
    }

    public override void OnCollisionEnter() {

    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

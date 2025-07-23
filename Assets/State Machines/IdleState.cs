using UnityEngine;

public class IdleState : PlayerState {
    public IdleState(Player player, PlayerStateMachine state) : base(player, state)
    { }

    public override void OnEnter() {
    }

    public override void Update() {
        if (player.GetAxisDirections().magnitude > 0) {
            stateMachine.ChangeStateTo(player._run_state);
        }

        if (Input.GetKeyDown(KeyCode.Space) && player._controller._colldata.below)
        {
            stateMachine.ChangeStateTo(player._jump_state);
        }
    }

    public override void FixedUpdate() {

    }

    public override void OnCollisionEnter() {

    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

using UnityEngine;

public class IdleState : PlayerState {

    private bool _jumpRequest;

    public IdleState(yuo player, PlayerStateMachine state) : base(player, state, PlayerStateList.Idle) {
    
    }

    public override void OnEnter() {
        player._velocity = Vector3.zero;
    }


    public override void FixedUpdate() {

        if (Mathf.Abs(player.GetAxisDirections().x) > 0) {
            stateMachine.ChangeStateTo(player._run_state);
            return;
        }
    }
}

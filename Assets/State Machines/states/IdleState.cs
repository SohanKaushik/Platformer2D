using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class IdleState : PlayerState {

    public IdleState(yuo player, PlayerStateMachine state) : base(player, state, PlayerStateList.Idle) {
    
    }

    public override void OnEnter() {
        player._velocity.x = 0f;
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            player.jumpRequest = true;
        }
    }

    public override void FixedUpdate() {


        // # jump 
        if(player.jumpRequest) {
            stateMachine.ChangeStateTo(player._jump_state);
            return;
        }

        // # run
        if (Mathf.Abs(player.GetAxisDirections().x) > 0) {
            stateMachine.ChangeStateTo(player._run_state);
            return;
        }
    }
}

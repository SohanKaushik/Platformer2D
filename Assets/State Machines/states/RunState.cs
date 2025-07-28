using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RunState : PlayerState
{
    private float _footSpeed;

    // [] Player -> Run State -> Player State ( protected variables )
    public RunState(Player player, PlayerStateMachine state, float speed) : base(player, state, PlayerStateList.Running) { 
        _footSpeed = speed;
    }

    public override void OnEnter()
    {
        // .. _animator.SetBool("run", true);
    }

    public override void Update()
    {
        // # jump
        if (Input.GetKeyDown(KeyCode.Space) && player.isGrounded()) {
            player.jumpRequest = true;
        }

        // # fall
        if (!player.isGrounded()) {
            stateMachine.ChangeStateTo(player._fall_state);
        }

        // # idle 
        if (Mathf.Abs(player.GetAxisDirections().x) == Mathf.Epsilon) { 
            stateMachine.ChangeStateTo(player._idle_state);
            return;
        }
    }

    public override void FixedUpdate()
    {
        // # jump 
        if (player.jumpRequest) {
            stateMachine.ChangeStateTo(player._jump_state);
            return;
        }

        player._velocity.x = player.GetAxisDirections().x * _footSpeed;
    }
}

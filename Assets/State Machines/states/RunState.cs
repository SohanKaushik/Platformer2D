using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RunState : PlayerState
{
    private float _footSpeed;
    private float _accelerationTimeGrounded = 0.05f;

    // [] Player -> Run State -> Player State ( protected variables )
    public RunState(Player player, PlayerStateMachine state, float speed, float accelerationTimeGrounded) : base(player, state, PlayerStateList.Running) { 
        _footSpeed = speed;
        _accelerationTimeGrounded = accelerationTimeGrounded;
    }

    public override void OnEnter()
    {
        // .. _animator.SetBool("run", true);
    }

    public override void Update()
    {
        // # dash
        if (player._context.dashRequest) {
            stateMachine.ChangeStateTo(player._dash_state);
            return;
        }

        // # jump 
        if ((player._context.jumpRequest && player.coyoteCounter >= 0.0f) || player.jumpBufferCounter > 0.0f) { 
            stateMachine.ChangeStateTo(player._jump_state);
            return;
        }

        // # idle 
        if (Mathf.Abs(player.GetAxisDirections().x) <= 0.1f) { 
            stateMachine.ChangeStateTo(player._idle_state);
            return;
        }
    }

    public override void FixedUpdate()
    {
        var targetvelocity = player.GetAxisDirections().x * _footSpeed;
        player._velocity.x = Mathf.SmoothDamp(player._velocity.x, targetvelocity, ref player._smooothfactorx, _accelerationTimeGrounded);
    }
}

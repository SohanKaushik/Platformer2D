using UnityEngine;

public class FallState : PlayerState
{
    private float _terminalMultiplier;
    private float _accelerationTimeAirborne = 0.05f;

    public FallState(Player player, PlayerStateMachine state, float terminalMultiplier, float accelerationTimeAirborne) : base(player, state, PlayerStateList.Falling)
    {
        _terminalMultiplier = terminalMultiplier;
        _accelerationTimeAirborne = accelerationTimeAirborne;
    }

    public override void Update()
    {
        // # run or idle
        if (player.isGrounded()) {

            if (Mathf.Abs(player.GetAxisDirections().x) > 0.1f) {
                stateMachine.ChangeStateTo(player._run_state);
            }
            else{
                stateMachine.ChangeStateTo(player._idle_state);
            }
            return;
        }

        // # jump 
        //if (player.jumpBufferCounter > 0.0f){
        //    stateMachine.ChangeStateTo(player._jump_state);
        //    return;
        //}

        if (player._context.jumpRequest && player.coyoteCounter >= 0.0f) {
            stateMachine.ChangeStateTo(player._jump_state);
            return;
        }
    }

    public override void FixedUpdate()
    {
        // # it has a depecdency to the jump Height and duration
        var targetvelocity = player.GetAxisDirections().x * player._footSpeed;
        player._velocity.x = Mathf.SmoothDamp(player._velocity.x, targetvelocity, ref player._smooothfactorx, _accelerationTimeAirborne);

        // # terminal velocity
        var fallSpeed = player._velocity.y + player._gravity * Time.fixedDeltaTime;
        player._velocity.y = Mathf.Max(fallSpeed, -_terminalMultiplier);

    }
}

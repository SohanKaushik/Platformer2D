using UnityEngine;

public class FallState : PlayerState
{
    private bool _jumpCut;
    private float _terminalMultiplier;
    private float _gravityModifier = 1.8f;
    private float _accelerationTimeAirborne = 0.05f;

    private float _wallInteractSpeed = 10.0f;
    private float targetvelocity;

    public FallState(Player player, PlayerStateMachine state, float terminalMultiplier, float accelerationTimeAirborne)
        : base(player, state, PlayerStateList.Falling)
    {
        _terminalMultiplier = terminalMultiplier;
        _accelerationTimeAirborne = accelerationTimeAirborne;
    }

    public override void Update()
    {
        //// # dash
        //if (player._context.dashRequest) {
        //    stateMachine.ChangeStateTo(player._dash_state);
        //    return;
        //}

        if (player.PlayerInputManager().OnJumpReleased() && player._velocity.y > 0f) {
            _jumpCut = true;
            return;
        }

        // # run or idle
        if (player.isGrounded()) {
            if (Mathf.Abs(player.GetAxisDirections().x) > 0.1f) {
                stateMachine.ChangeStateTo(player._run_state);
            }
            else {
                stateMachine.ChangeStateTo(player._idle_state);
            }
            return;
        }

        // # jump
        if (player.PlayerInputManager().OnJumpTapped() && player.coyoteCounter >= 0.0f) {
            stateMachine.ChangeStateTo(player._jump_state);
            return;
        }

        // # dash
        if (player.PlayerInputManager().OnDashTapped() && (player.IsDashAllowed() || player.jumpBufferCounter >= 0.0f)) {
            stateMachine.ChangeStateTo(player._dash_state);
            return;
        }

        // # wall jumping
        if (player.IsWallClimbAllowed())  {
            stateMachine.ChangeStateTo(player._wall_climb_state);
            return;
        }
    }

    public override void FixedUpdate()
    {
        var fallForce = 0f;

        float desired = player.GetAxisDirections().x * player._footSpeed;
        float airControlFactor = 0.5f; // tune between 0 (no control) and 1 (full control)

        // # cuts jump short if jump was released early
        if (_jumpCut) {
            player._velocity.y *= 0.5f;
            _jumpCut = false;
        }

        // # [0-peak] -> normal gravity :: [peak-ground] -> applied modified gravity
        if(player._velocity.y < 0.0f){
            fallForce = player._velocity.y + player._gravity * Time.fixedDeltaTime * _gravityModifier;


            if (player.IsCollided() && !player.isGrounded() && Mathf.Abs(player.GetAxisDirections().x) > 0.1f) {
                fallForce = -_wallInteractSpeed;
            }
            player._velocity.y = Mathf.Max(fallForce, -_terminalMultiplier);
            targetvelocity = Mathf.Lerp(player._velocity.x, desired, airControlFactor);
        }
        else {
            fallForce = player._velocity.y + player._gravity * Time.fixedDeltaTime;
            targetvelocity = desired;
            player._velocity.y = fallForce;
        }

        player._velocity.x = Mathf.SmoothDamp(player._velocity.x, targetvelocity, ref player._smooothfactorx, _accelerationTimeAirborne);
    }

    public override void OnExit()
    {
        // # prevents accumulated y velocity to any other state
        player._velocity.y = 0.0f;
        player.GetComponent<SpriteRenderer>().color = Color.white;
    }
}

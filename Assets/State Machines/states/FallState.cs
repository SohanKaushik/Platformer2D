using UnityEngine;

public class FallState : PlayerState
{
    private bool _jumpCut;
    private float _terminalMultiplier;
    private float _gravityModifier = 1.8f;
    private float _accelerationTimeAirborne = 0.05f;

    private float _wallInteractSpeed = 10.0f;
    private float targetvelocity;

    float airControlFactor = 0.5f;

    public FallState(Player player, PlayerStateMachine state, float terminalMultiplier, float accelerationTimeAirborne)
        : base(player, state, PlayerStateList.Falling)
    {
        _terminalMultiplier = terminalMultiplier;
        _accelerationTimeAirborne = accelerationTimeAirborne;
    }

    private float _groundedBuffer = 0.05f; // 50 ms buffer
    private float _lastAirTime = 0f;

    public override void Update()
    {
        // # half jump
        if (player.PlayerInputManager().OnJumpReleased() && player._velocity.y > 0f) {
            _jumpCut = true;
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

    public override void PhysicsUpdate()
    {
        float gravityToApply = player._gravity;
        float desired = player.GetAxisDirections().x * player._footSpeed;
        float airControlFactor = 0.5f;

        // --- Jump cut ---
        if (_jumpCut && player._velocity.y > 0f)
        {
            player._velocity.y *= 0.5f;
            _jumpCut = false;
        }


        // stronger gravity when falling
        if (player._velocity.y < -1.0f)
        {
            gravityToApply *= _gravityModifier;

            // Wall sliding
            if (player.IsCollided())
            {
                if(!player.isGrounded() && Mathf.Abs(player.GetAxisDirections().x) > 0.1f) { 
                     player._velocity.y = Mathf.Max(player._velocity.y, -_wallInteractSpeed);
                }

                if (player._liftBoosted) {
                    player._liftBoosted = false;
                    player._velocity = Vector3.zero;
                } 
            }
        }

        // applied gravity
        player._velocity.y += gravityToApply * Time.deltaTime;

        // terminal velocity
        player._velocity.y = Mathf.Max(player._velocity.y, -_terminalMultiplier);

        // horizontal whatever
        if (player._liftBoosted)
        {
            targetvelocity = player._velocity.x;
        }
        else if (player._velocity.y < 0f)
        {
            targetvelocity = Mathf.Lerp(player._velocity.x, desired, airControlFactor);
        }
        else
        {
            targetvelocity = desired;
        }

        player._velocity.x = Mathf.SmoothDamp(player._velocity.x, targetvelocity, ref player._smooothfactorx, _accelerationTimeAirborne);
    }


    public override void LateUpdate()
    {
        // # run or idle
        if (player.isGrounded())
        {
            if (Mathf.Abs(player.GetAxisDirections().x) > 0.1f) {
                stateMachine.ChangeStateTo(player._run_state);
            }
            else {
                stateMachine.ChangeStateTo(player._idle_state);
            }
            return;
        }
    }

    public override void OnExit()
    {
        // # prevents accumulated y velocity to any other state
        player._velocity.y = -0.1f;
        player.GetComponent<SpriteRenderer>().color = Color.white;
    }
}

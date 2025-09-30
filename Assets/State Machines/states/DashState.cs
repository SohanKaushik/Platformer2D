using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DashState : PlayerState
{
    private bool _dashCompleted;
    private float _dashDuration;
    private float _dashForce;
    private CinemachineImpulseSource _impulseSource;

    private Vector2 _dashDirection;
    Vector2 _NoInputDirection;

    private GhostTrailRenderer _trailRenderer;

    public DashState(Player player, PlayerStateMachine state, float dashSpeed, float dashDuration)
        : base(player, state, PlayerStateList.Dashing)
    {
        _dashForce = dashSpeed;
        _dashDuration = dashDuration;
    }

    public override void OnEnter()
    {
        _impulseSource = player.GetComponent<CinemachineImpulseSource>();
        _trailRenderer = player.GetComponent<GhostTrailRenderer>();

        player.StartCoroutine(DashCoroutine());
        _trailRenderer.DrawTrail(_dashDuration);
    }

    public override void FixedUpdate()
    {
        if (player._isDashing)
        {
            var dash = _dashDirection * _dashForce;
            player._velocity = dash;
        }
    }

    private IEnumerator DashCoroutine()
    {
        player._isDashing = true;
        _impulseSource.GenerateImpulse(player.GetAxisDirections() * 0.5f);

        Vector2 inputDir = player.GetAxisDirections();

        // # no direction input case
        if(inputDir.magnitude < 0.1f) {
            if(!player.isGrounded()) _dashDirection = new Vector2(0,1);
            else _dashDirection = new Vector2(player.GetDireciton(), 0).normalized;
        }
        else _dashDirection = inputDir.normalized;

        yield return new WaitForSeconds(_dashDuration);

        if (player._velocity.y > 0f)
            player._velocity.y = player._velocity.y * 0.5f;
        stateMachine.ChangeStateTo(player._fall_state);
    }

    public override void OnExit()
    {
    }
}

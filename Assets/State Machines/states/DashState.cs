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
            player._velocity = _dashDirection * _dashForce;

            // idk if it actually works
            //if (player.IsCollided()) { 
            //    player._velocity = Vector3.zero;
            //    player._stateMachine.ChangeStateTo(player._fall_state);
            //    return;
            //}
        }
    }

    private IEnumerator DashCoroutine()
    {
        player._isDashing = true;
        _impulseSource.GenerateImpulse(player.GetAxisDirections() * 0.5f);

        Vector2 inputDir = player.GetAxisDirections();
        _dashDirection = inputDir.magnitude > 0.1f
            ? inputDir.normalized
            : new Vector2(player.GetDireciton(), 0).normalized;


        yield return new WaitForSeconds(_dashDuration);

        stateMachine.ChangeStateTo(player._fall_state);
    }

    public override void OnExit()
    {
    }
}

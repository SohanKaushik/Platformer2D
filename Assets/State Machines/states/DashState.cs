using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DashState : PlayerState
{
    private bool _dashCompleted;
    private float _dashDuration;
    private float _dashSpeed;
    private CinemachineImpulseSource _impulseSource;

    private Vector2 _dashDirection;

    public DashState(Player player, PlayerStateMachine state, float dashSpeed, float dashDuration)
        : base(player, state, PlayerStateList.Dashing)
    {
        _dashSpeed = dashSpeed;
        _dashDuration = dashDuration;
    }

    public override void OnEnter()
    {
        _impulseSource = player.GetComponent<CinemachineImpulseSource>();
        player.StartCoroutine(DashCoroutine());
        _dashCompleted = false;
    }

    public override void FixedUpdate()
    {
        if (player._isDashing)
        {
            player._velocity = _dashDirection * _dashSpeed;
        }
    }

    private IEnumerator DashCoroutine()
    {
        player._isDashing = true;
        _impulseSource.GenerateImpulse(player.GetAxisDirections());

        Vector2 inputDir = player.GetAxisDirections();
        _dashDirection = inputDir.magnitude > 0.1f
            ? inputDir.normalized
            : new Vector2(player.GetDireciton(), 0);


        yield return new WaitForSeconds(_dashDuration);

        stateMachine.ChangeStateTo(player._fall_state);
    }
}

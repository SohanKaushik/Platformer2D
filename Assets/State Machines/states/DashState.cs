using System.Collections;
using UnityEngine;

public class DashState : PlayerState
{
    private float _dashDuration;
    private float _dashSpeed;

    private bool _isDashing;
    private Vector2 _dashDirection;

    public DashState(Player player, PlayerStateMachine state, float dashSpeed, float dashDuration)
        : base(player, state, PlayerStateList.Dashing)
    {
        _dashSpeed = dashSpeed;
        _dashDuration = dashDuration;
    }

    public override void OnEnter()
    {
        if (_isDashing) return;
        player.StartCoroutine(DashCoroutine());
    }

    public override void FixedUpdate()
    {
        if (_isDashing)
        {
            player._velocity = _dashDirection * _dashSpeed;
        }
    }

    private IEnumerator DashCoroutine()
    {
        _isDashing = true;

        Vector2 inputDir = player.GetAxisDirections();
        _dashDirection = inputDir.magnitude > 0.1f
            ? inputDir.normalized
            : new Vector2(player._controller._colldata.direction, 0);


        yield return new WaitForSeconds(_dashDuration);

        _isDashing = false;

        stateMachine.ChangeStateTo(player._fall_state);
    }
}

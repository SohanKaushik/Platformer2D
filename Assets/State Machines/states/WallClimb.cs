using Unity.VisualScripting;
using UnityEngine;

public class WallClimbState : PlayerState
{
    private Vector2 _wallJumpForce = new Vector2(30, 40);

    private bool _wallJumped;
    private float _climbUpSpeed;
    private float _climbDownSpeed;


    private Vector2 _nudgePushAtEdge;
    private Vector2 direciton;
    bool facingRight;

    public WallClimbState(Player player, PlayerStateMachine state, PlayerStateList name, float climpUpSpeed, float climbDownSpeed, Vector2 wallHopOff)
        : base(player, state, name)
    {
        _climbUpSpeed = climpUpSpeed;
        _climbDownSpeed = climbDownSpeed;
        _wallJumpForce = wallHopOff;
    }

    public override void OnEnter()
    {
        if (!player.IsStandingOnPlatform()) {
            player._velocity = Vector3.zero;
        }
        _nudgePushAtEdge = new Vector2 (player.GetDireciton() * 10f, 20);
    }

    public override void Update()
    {
        // # jump
        if (player.PlayerInputManager().OnJumpTapped()) {
            _wallJumped = true;
        }

        if (!player.IsWallClimbAllowed()) {
            stateMachine.ChangeStateTo(player._fall_state);
            return;
        }

        if(player.wallClimbTimer < 0 ) { 
            player._wallClimbTimeout = true;

            player.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            stateMachine.ChangeStateTo(player._fall_state);
            return;
        }

        // # dash
        if (player.IsDashAllowed())
        {
            stateMachine.ChangeStateTo(player._dash_state);
            return;
        }
    }

    public override void FixedUpdate()
    {
        direciton = new Vector2((player.GetAxisDirections().x), (player.GetAxisDirections().y));

        var downwardForce = _climbDownSpeed * direciton.y;
        var upwardForce = _climbUpSpeed * direciton.y;

        if (_wallJumped) {
            _wallJumped = false;

            player._velocity = (Mathf.Abs(direciton.x) > 0.1) ?
                new Vector2(-player.GetDireciton() * _wallJumpForce.x, _wallJumpForce.y) :
                new Vector2(0, 70);
            return;
        }

        if (HasReachedClimbTopEdge()) {
            player._velocity = _nudgePushAtEdge;
            return;
        }
        player._velocity.y = (direciton.y > 0.1) ? 
               upwardForce:
               downwardForce;
    }

    private bool HasReachedClimbTopEdge()
    {
        Collider2D col = player.GetComponent<Collider2D>();
        if (col == null) return false;

        float distance = 0.08f;
        float offset = 0f;
        Vector2 direction = new Vector2(player.GetDireciton(), 0);
        int layerMask = LayerMask.GetMask("Obstacles");

        facingRight = player.GetDireciton() == 1;

        Vector2 top = facingRight
            ? new Vector2(col.bounds.max.x, col.bounds.max.y - offset)
            : new Vector2(col.bounds.min.x, col.bounds.max.y - offset);

        Vector2 bottom = facingRight
            ? new Vector2(col.bounds.max.x, col.bounds.min.y)
            : new Vector2(col.bounds.min.x, col.bounds.min.y);

        RaycastHit2D topHit = Physics2D.Raycast(top, direction, distance, layerMask);
        RaycastHit2D bottomHit = Physics2D.Raycast(bottom, direction, distance, layerMask);

        Debug.DrawRay(top, direction * distance, Color.cyan);
        Debug.DrawRay(bottom, direction * distance, Color.cyan);

        return (!topHit && bottomHit);
    }

}

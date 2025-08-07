using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WallClimbState : PlayerState
{
    private float wallCrawlSpeed = 5.0f;
    private Vector2 wallJumpForce = new Vector2(30, 40);

    private bool _wallJumped;
    private float wallClimbDuration;
    private int climbSpeed = 2;

    private Vector2 direciton;
    public WallClimbState(Player player, PlayerStateMachine state, PlayerStateList name, float duration) : base(player, state, name)
    {
        wallClimbDuration = duration;
    }

    public override void OnEnter()
    {
        player._velocity = Vector3.zero;
    }

    public override void Update()
    {
        // # jump
        if (player.PlayerInputManager().OnJumpTapped()) {
            _wallJumped = true;
        }


        if(player.wallClimbTimer < 0) { 
            player._wallClimbTimeout = true;

            player.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            stateMachine.ChangeStateTo(player._fall_state);
            return;
        }
    }

    public override void FixedUpdate()
    {
        direciton = new Vector2((player.GetAxisDirections().x), (player.GetAxisDirections().y));

        var downwardForce = 20.0f * direciton.y;
        var upwardForce = 5.0f * direciton.y;

        if (_wallJumped) {
            _wallJumped = false;

            player._velocity = (Mathf.Abs(direciton.x) > 0.1) ? 
                new Vector2(-player.GetDireciton() * wallJumpForce.x, wallJumpForce.y) :
                new Vector2(0, 70);
            return;
        }

        if (HasReachedClimbTopEdge())
        {
            player._velocity = new Vector3(
                                                player.GetDireciton() * 10,
                                                20,
                                                0);
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
        float offset = 1f;
        Vector2 direction = new Vector2(player.GetDireciton(), 0);
        int layerMask = LayerMask.GetMask("Obstacles");

        bool facingRight = player.GetDireciton() == 1;

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

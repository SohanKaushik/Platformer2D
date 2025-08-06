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

        Debug.Log(HasReachedClimbTopEdge());
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
       player._velocity.y = (direciton.y > 0.1) ? 
               upwardForce:
               downwardForce;
    }

    bool HasReachedClimbTopEdge()
    {
        float wallCheckDistance = 0.1f;
        float verticalOffset = 0.5f;

        // Origin: in front of player, slightly above their head
        Vector2 origin = (Vector2)player.transform.position + new Vector2((player.GetDireciton() == 1) ? wallCheckDistance : -wallCheckDistance, verticalOffset);

        // Ray direction: check horizontally to see if wall continues
        Vector2 direction = (player.GetDireciton() == 1) ? Vector2.right : Vector2.left;

        RaycastHit2D wallHit = Physics2D.Raycast(origin, direction, wallCheckDistance);

        Debug.DrawRay(origin, direction * wallCheckDistance, Color.red);

        return wallHit.collider == null; // No wall? Then it's the top edge.
    }


}

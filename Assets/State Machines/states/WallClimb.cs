using UnityEngine;

public class WallClimbState : PlayerState
{
    private float wallCrawlSpeed = 5.0f;
    private Vector2 wallJumpForce = new Vector2(30, 40);

    private bool _wallJumped;

    public WallClimbState(Player player, PlayerStateMachine state, PlayerStateList name) : base(player, state, name)
    {
    }

    public override void OnEnter()
    {
        player._velocity = Vector3.zero;
    }

    public override void Update()
    {
        if (Input.GetKeyUp(KeyCode.K)){
            player._context.wallClimbHoldRequest = false;
        }

        // # jump
        if (player._context.jumpRequest) {
            _wallJumped = true;
        }
    }

    public override void FixedUpdate()
    {
        if (_wallJumped) { 
            player._velocity = wallJumpForce;
            return;
        }
       player._velocity.y = player.GetAxisDirections().y * wallCrawlSpeed;
    }

    public override void OnExit()
    {
        _wallJumped = false;
    }
}

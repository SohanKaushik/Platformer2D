using UnityEngine;

public class WallClimbState : PlayerState
{
    private float wallCrawlSpeed = 5.0f;

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
            
        }
    }

    public override void FixedUpdate()
    {
       player._velocity.y = player.GetAxisDirections().y * wallCrawlSpeed;
    }
}

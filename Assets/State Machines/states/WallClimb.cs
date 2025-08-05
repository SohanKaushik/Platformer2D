using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
            //Debug.Log("holed");
            _wallJumped = true;
        }


        //if(player.wallClimbTimer < 0) { 
        //    player._wallClimbTimeout = true;

        //    player.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        //    stateMachine.ChangeStateTo(player._fall_state);
        //    return;
        //}
    }

    public override void FixedUpdate()
    {
        direciton = new Vector2((player.GetAxisDirections().x), (player.GetAxisDirections().y));

        var downwardForce = 20.0f * direciton.y;
        var upwardForce = 10.0f * direciton.y;

        if (_wallJumped) {
            _wallJumped = false;

            player._velocity = (Mathf.Abs(player.GetAxisDirections().x) > 0.1) ? 
                new Vector2(-player.GetDireciton() * wallJumpForce.x, wallJumpForce.y) :
                new Vector2(0, 20);
            return;
        }
       player._velocity.y = (direciton.y > 0.1) ? 
               upwardForce:
               downwardForce;
    }
}

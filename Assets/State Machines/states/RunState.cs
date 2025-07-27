using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RunState : PlayerState
{
    private float _footSpeed;

    // [] Player -> Run State -> Player State ( protected variables )
    public RunState(yuo player, PlayerStateMachine state, float speed) : base(player, state, PlayerStateList.Running) { 
        _footSpeed = speed;
    }

    public override void OnEnter()
    {
        // .. _animator.SetBool("run", true);
    }

    public override void Update()
    {

        // # idle 
        if (Mathf.Abs(player.GetAxisDirections().x) == 0) { 
            stateMachine.ChangeStateTo(player._idle_state);
            return;
        }
    }

    public override void FixedUpdate()
    {
       player._velocity.x = player.GetAxisDirections().x * _footSpeed;
    }
}

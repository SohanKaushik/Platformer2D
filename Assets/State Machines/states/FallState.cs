using Unity.Android.Gradle.Manifest;
using Unity.Cinemachine;
using UnityEngine;

public class FallState : PlayerState
{
    public FallState(Player player, PlayerStateMachine state) : base(player, state, PlayerStateList.Falling)
    {

    }

    public override void OnEnter()
    {
    }

    public override void Update()
    {
        if (player._controller.isGrounded()) {
            //stateMachine.ChangeStateTo(player._idle_state);
            Debug.Log("Grounded");
        }
    }
    public override void FixedUpdate()
    {
        player._velocity.x = player.GetAxisDirections().x * player._footSpeed;

        // # it has a depecdency to the jump Height and duration
        player._velocity.y += player._gravity * Time.fixedDeltaTime;
    }
}

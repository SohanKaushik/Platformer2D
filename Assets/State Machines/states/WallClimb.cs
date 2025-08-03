using UnityEngine;

public class WallClimbState : PlayerState
{
    public WallClimbState(Player player, PlayerStateMachine state, PlayerStateList name) : base(player, state, name)
    {
    }

    public override void OnEnter()
    {
        Debug.Log("wall sliding");
        player._velocity = Vector3.zero;
    }
}

using UnityEngine;

// # this is a base class
public abstract class PlayerState {

    protected Player player;
    protected PlayerStateMachine stateMachine;

    public PlayerState(Player player, PlayerStateMachine state) { 
        this.player = player;
        this.stateMachine = state;
    }

    public virtual void OnEnter() { }
    public virtual void Update() { }    
    public virtual void FixedUpdate() { }
    public virtual void OnCollisionEnter(){ }
    public virtual void OnExit() { }
}

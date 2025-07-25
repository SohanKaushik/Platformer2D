using UnityEngine;

// # this is a base class
public abstract class PlayerState {

    protected Player player;
    protected PlayerStateMachine stateMachine;
    public PlayerStateList _name;

    public PlayerState(Player player, PlayerStateMachine state, PlayerStateList name) { 
        this.player = player;
        this.stateMachine = state;
        this._name = name;
    }

    public virtual void OnEnter() { }
    public virtual void Update() { }    
    public virtual void FixedUpdate() { }
    public virtual void OnCollisionEnter(){ }
    public virtual void OnExit() { }
}

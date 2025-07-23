using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour
{

    public PlayerStateMachine _stateMachine;
    public Controller2D _controller;

    public float _footSpeed;

    [Header("Jump Settings")]
    [SerializeField] private float _maxJumpHeight;
    [SerializeField] private float _jumpDuration;

    // states
    public IdleState _idle_state;
    public RunState _run_state;
    public JumpState _jump_state;
    public FallState _fall_state;

    [HideInInspector]
    public Vector2 _velocity;
    public float _gravity = -9.80f;

    void Awake()
    {
        _stateMachine = new PlayerStateMachine();
        _controller = GetComponent<Controller2D>();

        _idle_state = new IdleState(this, _stateMachine);
        _run_state = new RunState(this, _stateMachine, _footSpeed);
        _jump_state = new JumpState(this, _stateMachine, _maxJumpHeight, _jumpDuration);
        _fall_state = new FallState(this, _stateMachine);
    }

    private void Start()
    {
        _stateMachine.StartState(_fall_state);    
    }

    void Update()
    {
        _stateMachine._currentState.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine._currentState.FixedUpdate();

        _controller.move(_velocity);
    }

  

    public Vector2 GetAxisDirections() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}

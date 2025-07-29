using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    public PlayerStateMachine _stateMachine;
    public Controller2D _controller;

    public float _footSpeed;

    [Header("Jump Settings")]
    [SerializeField] private float _maxJumpHeight;
    [SerializeField] private float _jumpDuration;
    [SerializeField] private float _terminalMultiplier;
    
    [SerializeField] float _accelerationTimeGrounded;
    [SerializeField] float _accelerationTimeAirborne;

    // states
    public IdleState _idle_state;
    public RunState _run_state;
    public JumpState _jump_state;
    public FallState _fall_state;

    [HideInInspector] public Vector3 _velocity;
    [HideInInspector] public float _gravity = -1.0f;

    [HideInInspector] public bool jumpRequest;
    [HideInInspector] public bool jumpReleased;
    public float _coyoteTime;
    private float _maxJumpVelocity;

    [HideInInspector] public float _coyoteTimer;
    [HideInInspector] public float _smooothfactorx;

    void Awake()
    {
        _stateMachine = new PlayerStateMachine();
        _controller = GetComponent<Controller2D>();



        _idle_state = new IdleState(this, _stateMachine);
        _run_state = new RunState(this, _stateMachine, _footSpeed, _accelerationTimeGrounded);
        _fall_state = new FallState(this, _stateMachine, _terminalMultiplier, _accelerationTimeAirborne);
    }

    private void Start()
    {

        // # applied gravity
        _gravity = -(2 * _maxJumpHeight) / Mathf.Pow(_jumpDuration, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity) * _jumpDuration;

        _jump_state = new JumpState(this, _stateMachine, _maxJumpHeight, _jumpDuration, _maxJumpVelocity);
        print("Gravity: [" + _gravity + "] || " + "Velocity: [" + _maxJumpVelocity + "]");
        _stateMachine.StartState(_fall_state);
    }

    private void Update()
    {
        // Jump input states
        jumpRequest = Input.GetButtonDown("Jump");
        //jumpHeld = Input.GetButton("Jump");
        jumpReleased = Input.GetButtonUp("Jump");



        if (!isGrounded()) {

            if (_controller._colldata.above) _velocity.y = 0.01f;
            _stateMachine.ChangeStateTo(_fall_state);
        }

        _coyoteTimer = (isGrounded()) ? _coyoteTime : _coyoteTimer -= Time.deltaTime;
        _stateMachine._currentState.Update();
    }

    void FixedUpdate()
    {
        _stateMachine._currentState.FixedUpdate();
        _controller.move(_velocity * Time.deltaTime);
    }

    public Vector2 GetAxisDirections()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public bool isGrounded() => _controller._colldata.below;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard"))
        {
            GameManager.instance.NotifyDeath();
            StartCoroutine(GameManager.instance.Respawn(1.10f, this.gameObject));
        }
    }
}

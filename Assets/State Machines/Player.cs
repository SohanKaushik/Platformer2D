using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [Header("Walk")]
    public float _footSpeed;
    [HideInInspector] public PlayerStateMachine _stateMachine;
    [HideInInspector] public Controller2D _controller;
    [SerializeField] float _accelerationTimeGrounded;


    [Header("Jump")]
    [SerializeField] private float _jumpDuration;
    [SerializeField] float _coyoteTime;
    [SerializeField] float jumpBufferTime;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _terminalVelocity;
    
    [SerializeField] float _accelerationTimeAirborne;

    [Header("Dash")]
    [SerializeField] float _dashSpeed;
    [SerializeField] float _dashDuration;

    // states
    public IdleState _idle_state;
    public RunState _run_state;
    public JumpState _jump_state;
    public FallState _fall_state;
    public DashState _dash_state;
    public WallClimbState _wall_climb_state;
    
    [HideInInspector] public Vector3 _velocity;
    [HideInInspector] public float _gravity = -1.0f;

    public struct PublicContext { 
        public bool jumpRequest;
        public bool jumpReleased;
        public bool dashRequest;
        public bool wallClimbRequest;
    } public PublicContext _context;

    [HideInInspector] public float coyoteCounter;
    [HideInInspector] public float _smooothfactorx;

    [HideInInspector] public float jumpBufferCounter;
    [HideInInspector] public bool wallClimbAllowed;

    private float _maxJumpVelocity;

    void Awake()
    {
        _stateMachine = new PlayerStateMachine();
        _controller = GetComponent<Controller2D>();


        _idle_state = new IdleState(this, _stateMachine);
        _dash_state = new DashState(this, _stateMachine, _dashSpeed, _dashDuration);
        _run_state = new RunState(this, _stateMachine, _footSpeed, _accelerationTimeGrounded);
        _wall_climb_state = new WallClimbState(this, _stateMachine, PlayerStateList.WallClimbing);
        _fall_state = new FallState(this, _stateMachine, _terminalVelocity, _accelerationTimeAirborne);
    }

    private void Start()
    {

        // # applied gravity
        _gravity = -(2 * _jumpHeight) / Mathf.Pow(_jumpDuration, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity) * _jumpDuration;


        _jump_state = new JumpState(this, _stateMachine, _jumpHeight, _jumpDuration, _maxJumpVelocity);
        print("Gravity: [" + _gravity + "] || " + "Velocity: [" + _maxJumpVelocity + "]");
        _stateMachine.StartState(_fall_state);
    }

    private void Update()
    {
        // Jump input states
        _context.jumpRequest = Input.GetButtonDown("Jump");
        _context.jumpReleased = Input.GetButtonUp("Jump");

        _context.dashRequest = Input.GetKeyDown(KeyCode.L);
        _context.wallClimbRequest = Input.GetKeyDown(KeyCode.J);
        _stateMachine._currentState.Update();


        if (!isGrounded() ) {
            if (_controller._colldata.above) _velocity.y = 0.01f;
            if(_stateMachine._currentState != _fall_state && !wallClimbAllowed)
            _stateMachine.ChangeStateTo(_fall_state);
        }

        // # totally hideous code here
        if (GameManager.instance.Notifications.death)
        {
            StartCoroutine(GameManager.instance.Respawn(0.4f, this.gameObject));
        }

        // [ Conditions ]
        coyoteCounter = (isGrounded()) ? _coyoteTime : coyoteCounter -= Time.deltaTime;
        jumpBufferCounter = (_context.jumpRequest) ? jumpBufferTime : jumpBufferCounter -= Time.deltaTime;
        wallClimbAllowed = (_controller._colldata.right || _controller._colldata.left) && !isGrounded() ? true : false;

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

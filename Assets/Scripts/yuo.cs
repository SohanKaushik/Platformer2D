using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

[RequireComponent(typeof(Controller2D))]
public class yuo : MonoBehaviour
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
    public Vector3 _velocity;
    [HideInInspector]
    public float _gravity = -1.0f;
    private bool _on_ground;

    private float _maxJumpVelocity;
    public bool jumpRequest;

    void Awake()
    {
        _stateMachine = new PlayerStateMachine();
        _controller = GetComponent<Controller2D>();



        _idle_state = new IdleState(this, _stateMachine);
        _run_state = new RunState(this, _stateMachine, _footSpeed);
        _fall_state = new FallState(this, _stateMachine);
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
        _stateMachine._currentState.Update();
        Debug.Log(isGrounded());
    }

    void FixedUpdate()
    {
        _stateMachine._currentState.FixedUpdate();
        _controller.move(_velocity * Time.fixedDeltaTime);
    }

    public Vector2 GetAxisDirections() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public bool isGrounded() => _controller._colldata.below;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard")) {
            GameManager.instance.NotifyDeath();
            StartCoroutine(GameManager.instance.Respawn(1.10f, this.gameObject));
        }
    }
}

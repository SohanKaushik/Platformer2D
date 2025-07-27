using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

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
    public Vector3 _velocity;
    [HideInInspector]
    public float _gravity = -1.0f;
    private bool _on_ground;

    private float _maxJumpVelocity;
    private bool jumpRequest;

    void Awake()
    {
        _stateMachine = new PlayerStateMachine();
        _controller = GetComponent<Controller2D>();

       

        //_idle_state = new IdleState(this, _stateMachine);
        //_run_state = new RunState(this, _stateMachine, _footSpeed);
        //_jump_state = new JumpState(this, _stateMachine, _maxJumpHeight, _jumpDuration, _maxJumpVelocity);
        //_fall_state = new FallState(this, _stateMachine);
    }

    private void Start() {

        // # applied gravity
        _gravity = -(2 * _maxJumpHeight) / Mathf.Pow(_jumpDuration, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity) * _jumpDuration;

        //print("Gravity: [" + _gravity + "] || " + "Velocity: [" + _maxJumpVelocity + "]");
        //_stateMachine.StartState(_idle_state);
    }

    private void Update() {
        //_stateMachine._currentState.Update();


        if (_controller._colldata.above || _controller._colldata.below) _velocity.y = 0.0f;
      

    }

    void FixedUpdate() {
        //_stateMachine._currentState.FixedUpdate();

        _velocity.y += _gravity * Time.fixedDeltaTime;
        _controller.move(_velocity * Time.fixedDeltaTime);
    }

    //public Vector2 GetAxisDirections() {
    //    return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //}

    //public bool isGrounded() => _controller._colldata.below;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Hazard")) {
    //        GameManager.instance.NotifyDeath();
    //        StartCoroutine(GameManager.instance.Respawn(1.10f, this.gameObject));
    //    }
    //}
}

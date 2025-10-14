using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Controller2D), typeof(PlayerInputSystem))]
public class Player : MonoBehaviour
{
    [Header("Walk")]
    public float _footSpeed;
    [HideInInspector] public PlayerStateMachine _stateMachine;
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


    [HideInInspector] public float coyoteCounter;
    [HideInInspector] public float _smooothfactorx;

    [HideInInspector] public float jumpBufferCounter;

    [HideInInspector] public bool _isDashing;

    [HideInInspector] public bool _wallClimbTimeout;
    [HideInInspector] public float wallClimbTimer;

    [Header("Lift Boost Caps")]
    [SerializeField] private float _liftXCap = 80f;
    [SerializeField] private float _liftYCap = -130f;

    #region variables
    [Header("Wall Climb")]
    public float wallClimbDuration;
    public float wallClimbUpSpeed;
    public float wallClimbDownSpeed;
    public Vector2 wallHopOffForce;

    private float _maxJumpVelocity;
    private Controller2D _controller;
    private PlayerInputSystem _inputhandler;

    public MovingPlatforms currentPlatform;
    public bool _liftBoosted;
    private int _last_facing;
    #endregion

    void Awake()
    {
        _stateMachine = new PlayerStateMachine();
        _controller = GetComponent<Controller2D>();
        _inputhandler = GetComponent<PlayerInputSystem>();

        _idle_state = new IdleState(this, _stateMachine);
        _dash_state = new DashState(this, _stateMachine, _dashSpeed, _dashDuration);
        _run_state = new RunState(this, _stateMachine, _footSpeed, _accelerationTimeGrounded);
        _wall_climb_state = new WallClimbState(this, _stateMachine, PlayerStateList.Wall_Climbing, wallClimbUpSpeed, wallClimbDownSpeed, wallHopOffForce);
        _fall_state = new FallState(this, _stateMachine, _terminalVelocity, _accelerationTimeAirborne);
    }

    private void Start()
    {
        // # applied gravity
        _gravity = -(2 * _jumpHeight) / Mathf.Pow(_jumpDuration, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity) * _jumpDuration;


        _jump_state = new JumpState(this, _stateMachine, _jumpHeight, _jumpDuration, _maxJumpVelocity);
        print("Gravity: [" + _gravity + "] , " + "Velocity: [" + _maxJumpVelocity + "]");
        _stateMachine.StartState(_fall_state);
    }

    private void Update()
    {
        if (IsRidingOnPlatform() || IsRidingPlatformSideways())
        {
            transform.position += currentPlatform.GetDeltaMovement();
        }

        // #1 - Early exit conditions first
        if (GameManager.instance.Notifications.death)
        {
            StartCoroutine(GameManager.instance.Respawn(0.4f, this.gameObject));
            return;
        }

        #region Buffers
        coyoteCounter = isGrounded() ? _coyoteTime : coyoteCounter - Time.deltaTime;
        jumpBufferCounter = PlayerInputManager().OnJumpTapped() ? jumpBufferTime : jumpBufferCounter - Time.deltaTime;
        wallClimbTimer = IsWallClimbing() ? wallClimbTimer - Time.deltaTime : wallClimbDuration;
        #endregion

        int facing = (Mathf.Abs(GetAxisDirections().x) > 0.1f &&
                     (_stateMachine._currentState._name != PlayerStateList.Dashing) &&
                     Mathf.Abs(_velocity.x) > 0.1f) ?
                     (int)Mathf.Sign(GetAxisDirections().x) : 0;
        _controller.SetFacings(facing);
      
        _stateMachine._currentState.Update();
        _stateMachine._currentState.PhysicsUpdate();
        _controller.move(_velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {

        _last_facing = GetFacings();
        if (!isGrounded())
        {
            if (IsTouchingCeiling()) _velocity.y = 0.01f;
            if (_stateMachine._currentState != _fall_state && !IsWallClimbAllowed() && !_isDashing)
                _stateMachine.ChangeStateTo(_fall_state);
        }
    
        _stateMachine._currentState.LateUpdate();
    }

    public Vector2 GetAxisDirections() => _inputhandler.GetMoveInput();
    public PlayerStateList GetCurrentState() => _stateMachine._currentState._name;
    public PlayerInputSystem PlayerInputManager() => _inputhandler;
    public int GetFacings() => _controller._colldata.facing;
    public bool isGrounded() => _controller.IsGrounded();

    public bool IsWallClimbAllowed()
    {
        return (_controller._colldata.right || _controller._colldata.left)
         && !_wallClimbTimeout
         && !_controller._colldata.ascendingSlope
         && PlayerInputManager().IsWallClimbHeld();
    }

    public bool IsDashAllowed()
    {
        return !_isDashing
            && !(_stateMachine._currentState == _dash_state)
            && PlayerInputManager().OnDashTapped();
    }

    public bool IsWallClimbing()
    {
        return (_stateMachine._currentState == _wall_climb_state) ? true : false;
    }

    public bool IsCollided()
    {
        return (_controller._colldata.right || _controller._colldata.left);
    }

    public bool IsTouchingCeiling()
    {
        return _controller._colldata.above;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard"))
        {
            GameManager.instance.NotifyDeath();
            StartCoroutine(GameManager.instance.Respawn(1.10f, this.gameObject));
        }
    }

    public bool IsRidingOnPlatform()
    {
        var offset = (currentPlatform) ? currentPlatform.GetDeltaMovement() : Vector3.zero;
        return _controller.CollideCheck<MovingPlatforms>(offset,Vector2.down, out currentPlatform, true);
    }

   public bool IsRidingPlatformSideways()
   {
        return _controller.CollideCheck<MovingPlatforms>(Vector3.zero, new Vector2(GetFacings(), 0), out currentPlatform);
   }

    public void LockFacings()
    {
        _controller.SetFacings(_last_facing);
    }

    public Vector3 LiftBoost
    {
        get
        {
            if (!currentPlatform) {
                return Vector3.zero; }

            Vector3 platformVelocity = currentPlatform.GetVelocity();
            Vector3 liftBoost = platformVelocity;

            if (Mathf.Abs(liftBoost.x) > _liftXCap)
                liftBoost.x = _liftXCap * Mathf.Sign(liftBoost.x);

            if (Mathf.Abs(liftBoost.y) > _liftYCap)
                liftBoost.y = _liftYCap;

            _liftBoosted = true;
            Debug.Log(liftBoost);
            return liftBoost;
        }
    }

    //................
    private void OnValidate()
    {
        _gravity = -(2 * _jumpHeight) / Mathf.Pow(_jumpDuration, 2);
        _maxJumpVelocity = Mathf.Abs(_gravity) * _jumpDuration;


        _jump_state = new JumpState(this, _stateMachine, _jumpHeight, _jumpDuration, _maxJumpVelocity);
    }
}
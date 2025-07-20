using System.Collections;
using Unity.Android.Gradle.Manifest;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public Controller2D _controller;

    [Header("Main")]
    [SerializeField] float _footSpeed;
    [SerializeField] float _coyoteTime;
    [SerializeField] float _restTime;

    private float _coyoteTimer;
    private float _restTimer;

    [Header("Jump")]
    [SerializeField] float _minJumpHeight;
    [SerializeField] float _maxJumpHeight;
    [SerializeField] float _jumpTimed;
    [SerializeField] float _jumpFalloff;
    [SerializeField] float _maxJumpFalloff;
    private float _minJumpVelocity;
    private float _maxJumpVelocity;

    [Header("Dash")]
    public float _dashSpeed;
    public float _dashDuration;
    public float _dashCooldown;
    private bool _dashAllowed;
    private bool _dashing;
    [SerializeField] private TrailRenderer _trailRenderer;

    [Header("Animation")]
    [SerializeField] Animator _animator;
    [SerializeField] float accelerationTimeAirborne = 0.2f;
    [SerializeField] float accelerationTimeGrounded = 0.1f;

    [Header("Wall Sliding")]
    public bool _wallSliding = false;

    [SerializeField] Vector2 _wallLeap;

    [SerializeField] Vector2 _wallJumpOff;
    [SerializeField] Vector2 _wallJumpClimb;
    [SerializeField] float _wallUnStickTime;
    private float _wallStickTimer;
    [SerializeField] float _wallSlidingSpeedMax;


    //privates
    [HideInInspector]
    public Vector3 _velocity;
    private Vector2 _input;

    private bool _jumpPressed;
    private bool _jumpReleased;
    private bool _dashRequest;

    private float _smooothfactorx;
    private float _gravity;


    void Start()
    {
        _controller = GetComponent<Controller2D>();
        _trailRenderer = GetComponent<TrailRenderer>();

        _gravity = -(2 * _maxJumpHeight) / Mathf.Pow(_jumpTimed,2);
        _maxJumpVelocity = Mathf.Abs(_gravity) * _jumpTimed;
        _minJumpVelocity = Mathf.Sqrt(2.0f * Mathf.Abs(_gravity) * _minJumpHeight);

        _coyoteTime = Mathf.Clamp(_coyoteTime, 0.001f, int.MaxValue);
        _restTimer = _restTime;

        _trailRenderer.time = _jumpTimed;
        print("Gravity: [" + _gravity + "] Jump Velocity: [" + _maxJumpVelocity + "] Foot Speed: " + _footSpeed);
    }

    private void Update()
    {
        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.anyKeyDown) {
            _animator.SetBool("_awake", true);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            _jumpPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            _jumpReleased = true;
        }

        if (Input.GetKeyUp(KeyCode.K)) {
            _dashRequest = true;
        }

        if (!Input.anyKey){
            _restTimer -= Time.deltaTime;
        } else { 
            _restTimer = _restTime; _animator.SetBool("_isAFK", false);
        }

        if (_restTimer <= 0.0f) {
            _animator.SetBool("_isAFK", true);
        }

        if (_controller._colldata.above || _controller._colldata.below) { 
            _velocity.y = 0.0f;
            _animator.SetBool("_isJumping", false) ;
        }

        if (GameManager.instance.Notifications.death) {
            StartCoroutine(GameManager.instance.Respawn(0.4f, this.gameObject));
        }
    }

    void FixedUpdate()
    {
        float targetvelocity = _input.x * _footSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetvelocity, ref _smooothfactorx, (_controller._colldata.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        _animator.SetFloat("_speed", Mathf.Abs(_input.x));
        _animator.SetFloat("_vertical", _input.y);

        // Coyote Time
        _coyoteTimer = (_controller._colldata.below) ? _coyoteTime : _coyoteTimer-=Time.fixedDeltaTime;


        // Jump:
        if (_jumpPressed)
        {
            if (_controller._colldata.below || _coyoteTimer >= 0.0f) {
                _velocity.y = _maxJumpVelocity;
                _coyoteTimer = 0.0f;
                _animator.SetBool("_isJumping", true);
            }

            if (_wallSliding) {

                if(_controller._colldata.direction == _input.x)
                {
                    // pushes player in opposite direction ( not to the other wall)
                    _velocity.x = -_controller._colldata.direction * _wallJumpClimb.x;
                    _velocity.y = _wallJumpClimb.y;
                }
                else if(_controller._colldata.direction == 0)
                {
                    _velocity.x = -_controller._colldata.direction * _wallJumpOff.x;
                    _velocity.y = _wallJumpOff.y;
                }
                else
                {
                    _velocity.x = -_controller._colldata.direction * _wallLeap.x;
                    _velocity.y += _wallLeap.y;
                }
                    _wallSliding = false;
            }
            _jumpPressed = false; // consume jump
        }

        if (_jumpReleased) {
            if (_velocity.y > _minJumpVelocity) {
                _velocity.y = _minJumpVelocity;
            }
            _jumpReleased = false; // consume jump
        }

        // Wall Jumping:
        if ((_controller._colldata.right || _controller._colldata.left) && !_controller._colldata.below && _velocity.y < 0)
        {
            _wallSliding = true;
            _animator.SetBool("_isWallClimbing", true);
            _velocity.y = Mathf.Max(_velocity.y, -_wallSlidingSpeedMax);
        }
        else { _wallSliding = false; _animator.SetBool("_isWallClimbing", false); }

        // TODO: Wall Sticking, Double Jump, Dash

        _velocity.y += _gravity * Time.fixedDeltaTime;
        _controller.move(_velocity * Time.fixedDeltaTime);
    }
}

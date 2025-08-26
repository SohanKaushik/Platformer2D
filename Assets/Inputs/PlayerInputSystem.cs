using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionsAsset;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction wallClimbAction;

    public struct RequestContext {
        public bool jumpHeld;
        public bool jumpRequest;
        public bool jumpReleased;

        public bool dashRequest;
        public bool wallClimbHoldRequest;
    } private RequestContext _context;


    private bool jumpPressedThisFrame;
    private bool jumpReleasedThisFrame;
    private bool jumpHeld;

    private void OnEnable()
    {
        var map  = inputActionsAsset.FindActionMap("Player");

        moveAction = map.FindAction("Move");
        jumpAction = map.FindAction("Jump");
        dashAction = map.FindAction("Dash");
        wallClimbAction = map.FindAction("WallClimb");

        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();
        wallClimbAction.Enable();

        jumpAction.started += OnJumpTapped;
        jumpAction.canceled += OnJumpReleased; 

        //dashAction.started += OnDashTapped;
        dashAction.started += OnDashTapped;
        dashAction.canceled += OnDashReleased;

        wallClimbAction.started += OnClimbStarted;
        wallClimbAction.canceled += OnClimbCanceled;
    }


    private void OnDisable()
    {
        jumpAction.started -= OnJumpTapped;
        jumpAction.canceled -= OnJumpReleased;

        dashAction.started -= OnDashReleased;
        dashAction.canceled -= OnDashReleased;

        wallClimbAction.started -= OnClimbStarted;
        wallClimbAction.canceled -= OnClimbCanceled;

        moveAction.Disable();
        jumpAction.Disable();
        dashAction.Disable();
        wallClimbAction.Disable();
    }

    public Vector2 GetMoveInput()
    {
        return moveAction.ReadValue<Vector2>();
    }

    // # jump
    private void OnJumpTapped(InputAction.CallbackContext context)
    {
        jumpPressedThisFrame = true;
        jumpHeld = true;
    }

    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        jumpReleasedThisFrame = true;
        jumpHeld = false;
    }

    // # dash
    private void OnDashTapped(InputAction.CallbackContext context) => _context.dashRequest = true;
    private void OnDashReleased(InputAction.CallbackContext context) => _context.dashRequest = false;


    // # wall climbing 
    private void OnClimbStarted(InputAction.CallbackContext context) => _context.wallClimbHoldRequest = true;
    private void OnClimbCanceled(InputAction.CallbackContext context) => _context.wallClimbHoldRequest = false;

    // # get;
    public bool IsWallClimbHeld() => _context.wallClimbHoldRequest;

    public bool OnJumpReleased() => jumpReleasedThisFrame;
    public bool OnJumpTapped() => jumpPressedThisFrame;
    public bool OnJumpHeld() => jumpHeld;

    
   
    //public bool OnDashHeld() => jumpHeld;
    public bool OnDashTapped() => _context.dashRequest;
    //public bool OnDashReleased() => _context.dashRequest;


    private void LateUpdate()
    {
        jumpPressedThisFrame = false;
        jumpReleasedThisFrame = false;

        _context.dashRequest = false;
    }
}

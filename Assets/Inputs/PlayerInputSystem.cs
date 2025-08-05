using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionsAsset;
    private InputAction moveAction;
    private InputAction jumpAction;
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
        wallClimbAction = map.FindAction("WallClimb");

        moveAction.Enable();
        jumpAction.Enable();
        wallClimbAction.Enable();

        jumpAction.started += OnJumpTapped;
        jumpAction.canceled += OnJumpReleased; 

        wallClimbAction.started += OnClimbStarted;
        wallClimbAction.canceled += OnClimbCanceled;
    }

    private void OnDisable()
    {
        jumpAction.started -= OnJumpTapped;
        jumpAction.canceled -= OnJumpReleased;

        wallClimbAction.started -= OnClimbStarted;
        wallClimbAction.canceled -= OnClimbCanceled;

        moveAction.Disable();
        jumpAction.Disable();
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

    // # wall climbing 
    private void OnClimbStarted(InputAction.CallbackContext context) => _context.wallClimbHoldRequest = true;
    private void OnClimbCanceled(InputAction.CallbackContext context) => _context.wallClimbHoldRequest = false;

    // # get ;
    public bool IsWallClimbHeld() => _context.wallClimbHoldRequest;

    public bool OnJumpReleased() => jumpReleasedThisFrame;
    public bool OnJumpTapped() => jumpPressedThisFrame;
    public bool OnJumpHeld() => jumpHeld;


    private void LateUpdate()
    {
        jumpPressedThisFrame = false;
        jumpReleasedThisFrame = false;
    }
}

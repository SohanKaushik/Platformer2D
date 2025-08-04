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
        public bool jumpRequest;
        public bool dashRequest;
        public bool wallClimbHoldRequest;
    } private RequestContext _context;

    private void OnEnable()
    {
        var map  = inputActionsAsset.FindActionMap("Player");

        moveAction = map.FindAction("Move");
        jumpAction = map.FindAction("Jump");
        wallClimbAction = map.FindAction("WallClimb");

        moveAction.Enable();
        jumpAction.Enable();
        wallClimbAction.Enable();

        jumpAction.performed += OnJump;
        jumpAction.canceled += OnJumpReleased; 

        wallClimbAction.started += OnClimbStarted;
        wallClimbAction.canceled += OnClimbCanceled;
    }

    private void OnDisable()
    {
        jumpAction.performed -= OnJump;
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
    private void OnJump(InputAction.CallbackContext context) => _context.jumpRequest = true;
    private void OnJumpReleased(InputAction.CallbackContext context) => _context.jumpRequest = false;

    // # wall climbing 
    private void OnClimbStarted(InputAction.CallbackContext context) => _context.wallClimbHoldRequest = true;
    private void OnClimbCanceled(InputAction.CallbackContext context) => _context.wallClimbHoldRequest = false;

    // # receivers ->
    public bool IsWallClimbHeld() => _context.wallClimbHoldRequest;
    public bool HasJumped() => _context.jumpRequest; 
}

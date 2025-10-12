// summary //
// this player controller has been followed by video made by sabastian lague.
//

using JetBrains.Annotations;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngineInternal;

public class Controller2D : RaycastController
{
    [SerializeField] LayerMask _layermask;
    private float _maxClimbAngle = 80f;
    private bool _is_grounded;

    private Vector2 _remainder;

    public override void Start()
    {
        base.Start();
        _remainder = Vector2.zero;
    }

    public void move(Vector3 velocity, bool standingOnPlatform = false)
    {
        UpdateRayOrigins();
        _colldata.reset();


        if (velocity.x != 0)
        {
            _colldata.facing = (int)Mathf.Sign(velocity.x);
        }

        // Update player state
        transform.rotation = Quaternion.Euler(0f, _colldata.facing == -1 ? 180f : 0f, 0f);

        MoveH(ref velocity);
        MoveY(ref velocity);

        _is_grounded = _colldata.below;
    }

    private void MoveY(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float remainingY = Mathf.Abs(velocity.y);

        int steps = Mathf.CeilToInt(remainingY / 0.05f);
        steps = Mathf.Clamp(steps, 1, 20);

        float stepY = (remainingY / steps) * directionY;

        for (int step = 0; step < steps; step++)
        {
            Vector3 stepVelocity = new Vector3(0, stepY, 0);

            if (CollideCheck<BoxCollider2D>(new Vector2(0, directionY), out _))
            {
                //transform.Translate(0, -Mathf.Abs(stepY) * directionY, 0);

                _colldata.above = directionY == 1;
                _colldata.below = directionY == -1;

                break;
            }

            transform.Translate(stepVelocity, Space.World);

            // If we couldn't move this step, stop
            if (Mathf.Abs(stepVelocity.y) < 0.001f) break;

            UpdateRayOrigins();
        }
    }

    private void MoveH(ref Vector3 velocity)
    {

        float totalMoveH = velocity.x;
        float stepX = totalMoveH / 100f;

        var direction = (_colldata.facing == 1) ? Vector2.right : Vector2.left;

        for (int step = 0; step < 100; step++)
        {
            transform.Translate(stepX * direction.x, 0, 0);


            if (CollideCheck<BoxCollider2D>(direction, out _))
            {
                transform.Translate(-Mathf.Abs(stepX), 0, 0);

                _colldata.right = direction.x == 1;
                _colldata.left = direction.x == -1;

                break;
            }

            UpdateRayOrigins();
        }
    }

    void AscendSlope(ref Vector3 velocity, float angle)
    {
        var _moveDistance = Mathf.Abs(velocity.x);
        var _climbVelocityY = Mathf.Sin(angle * Mathf.Deg2Rad) * _moveDistance;

        if (velocity.y <= _climbVelocityY)
        {
            velocity.y = _climbVelocityY;
            velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * _moveDistance * _colldata.facing;

            _colldata.ascendingSlope = true;
            _colldata.slopeAngle = angle;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        Vector2 origin = (_colldata.facing == -1) ? _origins.bottomRight : _origins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, Mathf.Infinity, _layermask);
        if (!hit) return;

        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (slopeAngle == 0 || slopeAngle > _maxClimbAngle) return;
        if (Mathf.Sign(hit.normal.x) == Mathf.Sign(_colldata.facing))
        {
            // hit distance < perpendicular distance
            if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
            {
                // constant speed throughout the slope
                var _speed = Mathf.Abs(velocity.x);

                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * _speed * _colldata.facing;
                velocity.y -= Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * _speed;

                _colldata.slopeAngle = slopeAngle;
                _colldata.descendingSlope = true;
            }
        }
    }

    public bool IsSlopeCaptured()
    {
        return (_colldata.ascendingSlope || _colldata.descendingSlope);
    }

    public bool IsStandingOnPlatform()
    {
        return _colldata.standing_on_platform;
    }

    public bool IsGrounded()
    {
        return _is_grounded;
    }

    internal void SetFacings(int dir)
    {
        if (dir != 0)
            _colldata.facing = dir;
    }

    public bool CollideCheck<T>(Vector2 direction, out T component, out RaycastHit2D hit, bool debug = false) where T : Component
    {
        component = null;
        Vector2 startOrigin;
        Vector2 spreadDirection;
        hit = new RaycastHit2D();

        switch (direction)
        {
            case Vector2 v when v == Vector2.up:
                startOrigin = _origins.topLeft;
                spreadDirection = Vector2.right;
                break;
            case Vector2 v when v == Vector2.down:
                startOrigin = _origins.bottomLeft;
                spreadDirection = Vector2.right;
                break;
            case Vector2 v when v == Vector2.right:
                startOrigin = _origins.bottomRight;
                spreadDirection = Vector2.up;
                break;
            case Vector2 v when v == Vector2.left:
                startOrigin = _origins.bottomLeft;
                spreadDirection = Vector2.up;
                break;
            default:
                return false;
        }

        float rayLength = 0.1f;

        for (int i = 0; i < vraycount; i++)
        {
            var origin = startOrigin + spreadDirection * (vraySpacing * i);
            RaycastHit2D rayHit = Physics2D.Raycast(origin, direction, rayLength, _layermask);

            if(debug)
                Debug.DrawRay(origin, direction * rayLength, Color.green);

            if (rayHit)
            {
                hit = rayHit;
                component = hit.transform.GetComponent<T>();
                if (component != null)
                    return true;
            }
        }
        return false;
    }

    public bool CollideCheck<T>(Vector2 direction, out T component, bool debug = false) where T : Component
    {
        return CollideCheck(direction, out component, out _, debug);
    }
}
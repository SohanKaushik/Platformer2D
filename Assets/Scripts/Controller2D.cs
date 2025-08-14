// summary//
// this player controller has been followed by video made by sabastian lague.//

using Mono.Cecil.Cil;
using UnityEngine;

public class Controller2D : RaycastController
{

    [SerializeField] LayerMask _layermask;

    private float _maxClimbAngle = 80f;


    public override void Start()
    {
        base.Start();
    }

    public void move(Vector3 velocity)
    {
        UpdateRayOrigins();
        _colldata.reset();

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        if (velocity.x != 0)
        {
            _colldata.direction = (int)Mathf.Sign(velocity.x);
        }

        HorizontalCollision(ref velocity);
        VerticalCollision(ref velocity);

        // [] Flip
        transform.rotation = Quaternion.Euler(0f, _colldata.direction == -1 ? 180f : 0f, 0f);

        transform.Translate(velocity, Space.World);
    }

    protected override void VerticalCollision(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float raylength = Mathf.Abs(velocity.y) + skinWidth;

        if (velocity.y == 0) directionY = -1f;
        for (int i = 0; i < vraycount; i++)
        {
            Vector2 rayo = (directionY == -1) ? _origins.bottomLeft : _origins.topLeft;
            rayo += Vector2.right * (vraySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up * directionY, raylength, _layermask);


            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                raylength = hit.distance;


                if (_colldata.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(_colldata.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                _colldata.above = directionY == 1;
                _colldata.below = directionY == -1;
            }

            Debug.DrawRay(rayo, Vector2.up * directionY * raylength, Color.green);
        }
    }

    protected override void HorizontalCollision(ref Vector3 velocity)
    {
        float directionX = _colldata.direction;
        float raylength = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            raylength = 2 * skinWidth;
        }

        for (int i = 0; i < hraycount; i++)
        {
            Vector2 rayo = (directionX == -1) ? _origins.bottomLeft : _origins.bottomRight;
            rayo += Vector2.up * (hraySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.right * directionX, raylength, _layermask);


            if (hit)
            {
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= _maxClimbAngle)
                {
                    float distanceToSlopeStart = 0f;
                    if (slopeAngle != _colldata.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    AscendSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }

                if (!_colldata.climbingSlope || slopeAngle > _maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    raylength = hit.distance;

                    // slopeAngle > _maxClimbAngle : hits a wall or something restrict y movemnet
                    if (_colldata.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(_colldata.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    _colldata.right = directionX == 1;
                    _colldata.left = directionX == -1;
                }
            }

            Debug.DrawRay(rayo, Vector2.right * directionX * raylength, Color.blue);
        }

    }

    void AscendSlope(ref Vector3 velocity, float angle)
    {
        var _moveDistance = Mathf.Abs(velocity.x);
        var _climbVelocityY = Mathf.Sin(angle * Mathf.Deg2Rad) * _moveDistance;

        if (velocity.y <= _climbVelocityY)
        {
            velocity.y = _climbVelocityY;
            velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * _moveDistance * _colldata.direction;

            _colldata.below = true;
            _colldata.climbingSlope = true;
            _colldata.slopeAngle = angle;
        }
    }
    void DescendSlope(ref Vector3 velocity)
    {
        Vector2 rayOrigin = (_colldata.direction == 1) ? _origins.bottomLeft : _origins.bottomRight;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, _layermask);

        //if (hit)
        //{
        //    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

        //    if (slopeAngle == 0 && slopeAngle > _maxClimbAngle) return;

        //    if (Mathf.Sign(hit.normal.x) == Mathf.Sign(_colldata.direction))
        //    {
        //        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
        //        {
        //            var _moveDistance = Mathf.Abs(velocity.x);
        //            var _descentVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * _moveDistance;

        //            velocity.y -= _descentVelocityY;
        //            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * _moveDistance * _colldata.direction;

        //            _colldata.slopeAngle = slopeAngle;
        //            _colldata.descendSlope = true;
        //            _colldata.below = true;
        //        }
        //    }
        //}
    }
}


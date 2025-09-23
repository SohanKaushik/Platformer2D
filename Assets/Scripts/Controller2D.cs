// summary //
// this player controller has been followed by video made by sabastian lague.
//

using UnityEngine;

public class Controller2D : RaycastController
{

    [SerializeField] LayerMask _layermask;
    private float _maxClimbAngle = 80f;
    private bool _is_grounded;

    public override void Start()
    {
        base.Start();
    }

    public void move(Vector3 velocity, bool standing_on_platform = false)
    {
        UpdateRayOrigins();
        _colldata.reset();

        if (velocity.x != 0)
        {
            _colldata.direction = (int)Mathf.Sign(velocity.x);
        }

        if (velocity.y <= 0 || !_colldata.below)
        {
            DescendSlope(ref velocity);
        }

        VerticalCollision(ref velocity);
        HorizontalCollision(ref velocity);


        // [] Flip
        transform.rotation = Quaternion.Euler(0f, _colldata.direction == -1 ? 180f : 0f, 0f);

        _is_grounded = _colldata.below;
        transform.Translate(velocity, Space.World);
        if (standing_on_platform) _is_grounded = _colldata.below = true;
    }

   
    protected void VerticalCollision(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        if (velocity.y == 0) directionY = -1f;

        bool firstHit = false, lastHit = false;

        for (int i = 0; i < vraycount; i++)
        {
            Vector2 rayo = (directionY == -1) ? _origins.bottomLeft : _origins.topLeft;
            rayo += Vector2.right * (vraySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up * directionY, rayLength, _layermask);

            if (hit && hit.distance >= 0)
            {
                _colldata.above = directionY == 1;
                _colldata.below = directionY == -1;

                if (i == 0 && _colldata.above) firstHit = true;
                if (i == vraycount - 1 && _colldata.above) lastHit = true;
            }
            Debug.DrawRay(rayo, Vector2.up * directionY * rayLength, Color.green);
        }

        // # edge detection
        // # preserving vertical velocity when edge collided
        if (firstHit ^ lastHit)
        {
            velocity.x += (firstHit ? +0.75f : -0.75f);
        }
        else
        {
            for (int i = 0; i < vraycount; i++)
            {
                Vector2 rayo = (directionY == -1) ? _origins.bottomLeft : _origins.topLeft;
                rayo += Vector2.right * (vraySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up * directionY, rayLength, _layermask);

                if (!hit || hit.distance == 0) continue;
                
                    velocity.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    if (_colldata.ascendingSlope)
                    {
                        velocity.x = velocity.y / Mathf.Tan(_colldata.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }

                    _colldata.above = directionY == 1;
                    _colldata.below = directionY == -1;

                    if (_colldata.above) _colldata.ceilingHit = true;
                    Debug.DrawRay(rayo, Vector2.up * directionY * rayLength, Color.red);
                
            }
        }
    }


    protected void HorizontalCollision(ref Vector3 velocity)
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


            if (!hit || hit.distance == 0) continue;

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

            if (!_colldata.ascendingSlope || slopeAngle > _maxClimbAngle)
            {
                velocity.x = (hit.distance - skinWidth) * directionX;
                raylength = hit.distance;

                // slopeAngle > _maxClimbAngle : hits a wall or something restrict y movemnet
                if (_colldata.ascendingSlope)
                {
                    velocity.y = Mathf.Tan(_colldata.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                }

                _colldata.right = directionX == 1;
                _colldata.left = directionX == -1;
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

            _colldata.ascendingSlope = true;
            _colldata.slopeAngle = angle;
        }
    }
    void DescendSlope(ref Vector3 velocity)
    {
        Vector2 origin = (_colldata.direction == -1) ? _origins.bottomRight : _origins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, Mathf.Infinity, _layermask);
        if (!hit) return;

        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (slopeAngle == 0 || slopeAngle > _maxClimbAngle) return;
        if (Mathf.Sign(hit.normal.x) == Mathf.Sign(_colldata.direction))
        {

            // hit distance < perpendicular distance
            if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
            {

                // constant speed throughout the slope
                var _speed = Mathf.Abs(velocity.x);

                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * _speed * _colldata.direction;
                velocity.y -= Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * _speed;

                _colldata.slopeAngle = slopeAngle;
                _colldata.descendingSlope = true;
            }
        }
    }


    public bool isSlopeCaptured()
    {
        return (_colldata.ascendingSlope || _colldata.descendingSlope);
    }

    public bool isRidingPlatform(bool riding)
    {
        return riding;
    }

    public bool IsGrounded()
    {
        return _is_grounded;
    }
}
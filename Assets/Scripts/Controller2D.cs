using NUnit.Framework;
using System;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class Controller2D : RaycastController {

    [SerializeField] LayerMask _layermask;

    public override void Start() {
        base.Start();    
    }

    public void move(Vector3 velocity)
    {
        UpdateRayOrigins();
        _colldata.reset();


        if(velocity.x != 0) {
            _colldata.direction = (int)Mathf.Sign(velocity.x);
        }

        HorizontalCollision(ref velocity);
        VerticalCollision(ref velocity);
        if (velocity.y !< 0.1) {
        }

        // [] Flip
        transform.rotation = (_colldata.direction == -1) ?
            Quaternion.Euler(transform.rotation.x, 180f, transform.rotation.z) : Quaternion.Euler(transform.rotation.x, 0f, transform.rotation.z);

        transform.Translate(velocity, Space.World);
    }

    protected override void VerticalCollision(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float raylength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < vraycount; i++)
        {
            Vector2 rayo = (directionY == -1) ? _origins.bottomLeft : _origins.topLeft;
            rayo += Vector2.right * (vraySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up * directionY, raylength, _layermask);


            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                raylength = hit.distance;

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

        if (velocity.x < skinWidth)
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
                velocity.x = (hit.distance - skinWidth) * directionX;
                raylength = hit.distance;

                _colldata.right = directionX == 1;
                _colldata.left = directionX == -1;
            }

            Debug.DrawRay(rayo, Vector2.right * directionX * raylength, Color.blue);
        }
    }
}


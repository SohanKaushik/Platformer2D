using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    public LayerMask collisionMask;
    public const float skinWidth = 0.015f;


    public int vraycount = 4;
    public int hraycount = 4;

    public RayOrigins _origins;
    public CollisionData _colldata;

    [HideInInspector]
    public BoxCollider2D _collider;

    [HideInInspector]
    public float vraySpacing, hraySpacing;

    public virtual void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _colldata.facing = 1;

        CalculateRaySpacing();
    }
    public void UpdateRayOrigins()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2f);

        _origins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _origins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _origins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _origins.topRight = new Vector2(bounds.max.x, bounds.max.y);

        _origins.topCentre = new Vector2(_origins.topLeft.x/2, _origins.topRight.y /2);
        _origins.bottomCentre = new Vector2(_origins.bottomLeft.x / 2, _origins.bottomRight.y / 2);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2f);

        vraycount = Mathf.Clamp(vraycount, 2, int.MaxValue);
        hraycount = Mathf.Clamp(hraycount, 2, int.MaxValue);

        vraySpacing = bounds.size.x / (vraycount - 1);
        hraySpacing = bounds.size.y / (hraycount - 1);
    }

    public struct RayOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
        public Vector2 topCentre, bottomCentre;
    }

    public struct CollisionData
    {
        public bool below, above;
        public bool right, left;
        public int facing;

        public bool moving;
        public bool ascendingSlope, descendingSlope;
        public float slopeAngle, slopeAngleOld;

        public bool ceilingHit;
        public bool standing_on_platform;

        public void reset()
        {
            right = left = false;
            below = above = false;

            moving = false;
            ceilingHit = false;

            ascendingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0f;

            standing_on_platform = false;
        }
    }
}

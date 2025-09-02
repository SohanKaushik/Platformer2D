using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    protected int vraycount = 4;
    protected int hraycount = 4;

    public RayOrigins _origins;
    public CollisionData _colldata;

    private BoxCollider2D _collider;
    protected float skinWidth = 0.015f;
    protected float vraySpacing, hraySpacing;

    public virtual void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _colldata.direction = 1;
    }

    private void Update()
    {
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
    }

    public struct CollisionData
    {
        public bool below, above;
        public bool right, left;
        public int direction;

        public bool ascendingSlope, descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public void reset()
        {
            right = left = false;
            below = above = false;

            ascendingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0f;
        }
    }
}

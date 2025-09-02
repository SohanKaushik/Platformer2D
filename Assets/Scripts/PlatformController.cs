using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlatformController : RaycastController {

    private Vector3 move;
    [SerializeField] LayerMask passangerMask;

    public override void Start()
    {
        base.Start();

    }

    private void Update()
    {
        UpdateRayOrigins();

        Vector3 velocity = move * Time.deltaTime;

        MovePassangers(velocity);
        transform.Translate(velocity);
    }

    void MovePassangers(Vector3 velocity) {

        var directions = new Vector2(Mathf.Sign(velocity.x), Mathf.Sign(velocity.y));

        // # vertical
        if (Mathf.Abs(velocity.y) < 0.1) return;

        for (int i = 0; i < vraycount; i++)
        {
            var rayLength = Mathf.Abs(velocity.y) + skinWidth;
            var rayo = (directions.y == 1) ? _origins.topLeft : _origins.bottomLeft;
            rayo += Vector2.right * (vraySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up * directions.y, rayLength, passangerMask);

            if (hit)
            {
                var pushY = transform.position.y - (hit.distance - skinWidth) * directions.y;
                var pushX = (directions.y == 1) ? velocity.x : 0;
                transform.Translate(new Vector3(pushX, pushY));
            }
        }
    }
}

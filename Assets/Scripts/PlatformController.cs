using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlatformController : RaycastController {

    private Vector3 move = Vector3.up * 1.0f;
    [SerializeField] LayerMask passangerMask;


    public override void Start() {
        base.Start();
    }

    private void Update()
    {
        UpdateRayOrigins();
        _colldata.reset();

        Vector3 velocity = move * Time.deltaTime;

        MovePassangers(velocity);
        transform.Translate(velocity);
    }

    void MovePassangers(Vector3 velocity) {

        var directions = new Vector2(Mathf.Sign(velocity.x), Mathf.Sign(velocity.y));

        // # vertical
        if (Mathf.Abs(velocity.y) < 0.1)
        {
            var rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < vraycount; i++)
            {
                var rayo = (directions.y == 1) ? _origins.topLeft : _origins.bottomLeft;
                rayo += Vector2.right * (vraySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up * directions.y, rayLength, passangerMask);

                if (hit)
                {
                    var pushY = velocity.y - (hit.distance - skinWidth) * directions.y;
                    var pushX = (directions.y == 1) ? velocity.x : 0;

                    hit.transform.Translate(new Vector3(pushX, pushY));
                }

                Debug.DrawRay(rayo, Vector3.up * directions.y, Color.red);
            }

        }
    }
}

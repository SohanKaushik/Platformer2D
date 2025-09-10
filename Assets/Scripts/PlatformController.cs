using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlatformController : RaycastController
{

    [SerializeField] Vector3 move = Vector3.up * 0.7f;
    [SerializeField] LayerMask passangerMask;
    private Vector3 origin;

    public override void Start()
    {
        base.Start();

        origin = transform.position;
    }

    private void FixedUpdate()
    {
        UpdateRayOrigins();
        _colldata.reset();

        Vector3 velocity = move * Time.fixedDeltaTime;

        MovePassangers(velocity);
        transform.Translate(velocity);

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = origin;
        }
    }

    void MovePassangers(Vector3 velocity)
    {
        var player = GetComponent<Player>();
        var directions = new Vector2(Mathf.Sign(velocity.x), Mathf.Sign(velocity.y));

        // # vertical
        if (velocity.y != 0f)
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
                    //hit.transform.gameObject.GetComponent<Controller2D>()._colldata.moving = true;
                }

                Debug.DrawRay(rayo, Vector3.up * directions.y, Color.red);
            }

        }

        // horizontal
        if (velocity.x != 0f)
        {
            float raylength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < hraycount; i++)
            {
                Vector2 rayo = (directions.x == -1) ? _origins.bottomLeft : _origins.bottomRight;
                rayo += Vector2.up * (hraySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up * directions.y, raylength, passangerMask);

                if (hit)
                {
                    var pushX = velocity.x - (hit.distance - skinWidth) * directions.x;
                    var pushY = 0;

                    hit.transform.Translate(new Vector3(pushX, pushY));
                    //hit.transform.gameObject.GetComponent<Controller2D>()._colldata.moving = true;
                }

                Debug.DrawRay(rayo, Vector3.up * directions.y, Color.red);
            }
        }

        // 
        if (directions.y == -1 || velocity.y == 0.0f || player.IsWallClimbing() && directions.x != 0.0f) {
            var rayLength = skinWidth * 2f;
            for (int i = 0; i < vraycount; i++)
            {
                Debug.Log("jnasn");
                var rayo = _origins.topLeft + Vector2.right * (vraySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up, rayLength, passangerMask);

                if (hit)
                {
                    var pushY = velocity.y;
                    var pushX = velocity.x;

                    Debug.Log(pushX);
                    hit.transform.Translate(new Vector3(pushX, pushY));
                    hit.transform.gameObject.GetComponent<Player>().onPlatform = true;
                }
                Debug.DrawRay(rayo, Vector3.up * directions.y, Color.red);
            }
        }
    }
}
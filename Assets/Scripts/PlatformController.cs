using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlatformController : RaycastController
{

    [SerializeField] Vector3 move = Vector3.up;
    [SerializeField] LayerMask passangerMask;
    private Vector3 origin;

    PassangersData _passanger = new PassangersData();
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
        EvaluatePassangerMovement(velocity);

        MovePassanger(true);
        transform.Translate(velocity);
        MovePassanger(false);

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = origin;
        }
    }

    void MovePassanger(bool beforeMovePlatform) {

        if (!_passanger.transform) return;
        if (_passanger.moveBeforePlatform == beforeMovePlatform) {
            _passanger.transform.GetComponent<Controller2D>().move(_passanger.velocity);
            _passanger = default;
        }
    }

    void EvaluatePassangerMovement(Vector3 velocity)
    {
        var directions = new Vector2(Mathf.Sign(velocity.x), Mathf.Sign(velocity.y));

        //
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
                    _passanger = new PassangersData(hit.transform, new Vector3(pushX, pushY), directions.y == 1, true);
                }

                Debug.DrawRay(rayo, Vector3.up * directions.y, Color.red);
            }

        }

        // horizontal
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < hraycount; i++)
            {
                Vector2 rayOrigin = (directions.x == -1) ? _origins.bottomLeft : _origins.bottomRight;
                rayOrigin += Vector2.up * (hraySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directions.x, rayLength, passangerMask);

                if (hit)
                {
                    float pushX = velocity.x - (hit.distance - skinWidth) * directions.x;
                    float pushY = 0;

                   _passanger = new PassangersData(hit.transform, new Vector3(pushX, pushY), false, true);
                 }
            }
        }


        // 
        if (directions.y == -1 || (velocity.y == 0 && velocity.x != 0))
        {
            var rayLength = skinWidth * 2f;

            for (int i = 0; i < vraycount; i++)
            {
                Vector2 rayo = _origins.topLeft + Vector2.right * (vraySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up, rayLength, passangerMask);

                if (hit)
                {
                    var pushY = velocity.y;
                    var pushX = velocity.x;
                    _passanger = new PassangersData(hit.transform, new Vector3(pushX, pushY), true, false);
                }
                Debug.DrawRay(rayo, Vector2.up, Color.white);
            }
        }

    }

    struct PassangersData {
        public Transform transform; 
        public Vector3 velocity;
        public bool onPlatform;
        public bool moveBeforePlatform;

        public PassangersData(Transform _transform, Vector3 _velocity, bool _onPlatform, bool _moveBeforePlatformm) {
            transform = _transform;
            velocity = _velocity;
            onPlatform = _onPlatform;
            moveBeforePlatform = _moveBeforePlatformm;
        }
    }
}
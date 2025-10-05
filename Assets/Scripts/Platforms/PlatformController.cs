using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public abstract class PlatformController : RaycastController
{
    public LayerMask passengerMask;
    //public Collider2D triggerShape2D;

    [SerializeField] AnimationCurve _easing;
    public float _speed;

    PassangersData _passanger = new PassangersData();

    private CinemachineImpulseSource _impulseSource;
    protected Vector3 _original_position;

    public override void Start()
    {
        base.Start();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        UpdateRayOrigins();
    }

    protected float Ease(float t)
    {
        t = Mathf.Clamp01(t);
        return _easing.Evaluate(t);
    }

    public abstract Vector3 EvaluatePlatformMovement();
    public abstract float EvaluatePassengerMovement();
    public abstract void MovePassengers(Vector3 dv);

    //void MovePassanger(bool beforeMovePlatform)
    //{

    //    if (!_passanger.transform) return;
    //    if (_passanger.moveBeforePlatform == beforeMovePlatform)
    //    {

    //        var _player = _passanger.transform.GetComponent<Player>();
    //        if (_player._stateMachine._currentState._name == PlayerStateList.Jumping) {
    //            OnPlayerJump(_player);
    //        }
    //        _passanger.transform.GetComponent<Controller2D>().move(_passanger.velocity, _passanger.onPlatform);
    //    }
    //}
    void EvaluatePassangerMovement(Vector3 velocity)
    {
        _passanger = default;
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);


        // # vertical
        if (velocity.y != 0f)
        {
            var rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < vraycount; i++)
            {
                var rayo = (directionY == 1) ? _origins.topLeft : _origins.bottomLeft;
                rayo += Vector2.right * (vraySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up * directionY, rayLength, passengerMask);

                if (hit)
                {
                    var pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                    var pushX = (directionY == 1) ? velocity.x : 0;
                    _passanger = new PassangersData(hit.transform, new Vector3(pushX, pushY), directionY == 1, true);
                    break;
                }

                //Debug.DrawRay(rayo, Vector3.up * rayLength * directionY, Color.red);
            }
        }

        // horizontal
        //HorizontalCollision(directionX);

        // downwards or sideways on top
        // Passenger on top (moving down or horizontally)
        if (directionY == -1 || velocity.y == 0 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < vraycount; i++)
            {
                Vector2 rayOrigin = _origins.topLeft + Vector2.right * (vraySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                if (hit)
                {
                    float pushX = velocity.x;
                    float pushY = velocity.y;

                    _passanger = new PassangersData(hit.transform, new Vector3(pushX, pushY), true, false);
                }
               // Debug.DrawRay(rayOrigin, Vector3.up * rayLength, Color.white);
            }
        }
    }

    struct PassangersData
    {
        public Transform transform;
        public Vector3 velocity;
        public bool onPlatform;
        public bool moveBeforePlatform;

        public PassangersData(Transform _transform, Vector3 _velocity, bool _onPlatform, bool _moveBeforePlatformm)
        {
            transform = _transform;
            velocity = _velocity;
            onPlatform = _onPlatform;
            moveBeforePlatform = _moveBeforePlatformm;
        }
    }

    struct PlatformData
    {
        public float speed;
        public int fromWayPointIndex;
        public float percentageBetweenWaypoints;
    }

   
    //public void OnPlayerJump(Player player)
    //{
    //    if (boost_buffer >= 0) {
    //        player.ApplyPlatformBoost(1, _boost * new Vector3(0, 1, 0));
    //    }
    //    else player.ApplyPlatformBoost(_platform.percentageBetweenWaypoints, _boost * velocity);
    //}
    //private bool IsPlatformReturned() {
    //    if (_platform.fromWayPointIndex == 0 && Vector3.Distance(transform.position, _origin_pos) <= 0.001f) {
    //        transform.position = _globalWaypoints[0];
    //        return true;
    //    }
    //    return false;
    //}

    //private Collider2D DetectedPassenger()
    //{
    //    ContactFilter2D filter = new ContactFilter2D();
    //    filter.SetLayerMask(passengerMask);
    //    filter.useTriggers = true;

    //    Collider2D[] results = new Collider2D[3];

    //    int count = triggerShape2D.Overlap(filter, results);
    //    return count > 0 ? results[0] : null;
    //}


    //private void HorizontalCollision(float direction)
    //{
    //    float rayLength = Mathf.Abs(velocity.x) + skinWidth;

    //    for (int i = 0; i < hraycount; i++)
    //    {
    //        Vector2 leftRayOrigin = _origins.bottomLeft + Vector2.up * (hraySpacing * i);
    //        Vector2 rightRayOrigin = _origins.bottomRight + Vector2.up * (hraySpacing * i);

    //        RaycastHit2D left_hit = Physics2D.Raycast(leftRayOrigin, Vector2.left, rayLength, passengerMask);
    //        RaycastHit2D right_hit = Physics2D.Raycast(rightRayOrigin, Vector2.right, rayLength, passengerMask);

    //        RaycastHit2D hit = left_hit ? left_hit : right_hit;

    //        if (hit && hit.distance <= 0.01f) {
    //            GameManager.instance.NotifyDeath();
    //           break;
    //        }

    //        if (hit) {

    //            float pushX = 0f;
    //            float pushY = 0f;

    //            if(hit.transform.GetComponent<Player>().IsWallClimbing()) {
    //                pushX = velocity.x;
    //                _passanger = new PassangersData(hit.transform, new Vector3(pushX, pushY), false, (left_hit && direction == -1));
    //            }
    //            else
    //            {
    //                pushX = velocity.x - (hit.distance - skinWidth) * direction;
    //                _passanger = new PassangersData(hit.transform, new Vector3(pushX, pushY), false, true);
    //            }

    //        }

    //        Debug.DrawRay(leftRayOrigin, Vector2.left, Color.red);
    //        Debug.DrawRay(rightRayOrigin, Vector2.right, Color.red);
    //    }

    //}
}
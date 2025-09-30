using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class PlatformController : RaycastController
{

    [SerializeField] LayerMask passengerMask;

    [SerializeField]
    Vector3[] _localWaypoints;
    Vector3[] _globalWaypoints;

    [SerializeField] AnimationCurve _easing;

    PassangersData _passanger = new PassangersData();
    PlatformData _platform = new PlatformData();

    [SerializeField] float _speed;
    [SerializeField] float _boost;

    [SerializeField] float _waitTime;
    [SerializeField] bool _cyclic;
    private float _nextMoveTime;

    private Vector3 velocity;
    private float boost_buffer = 0.3f;

    private CinemachineImpulseSource _impulseSource;

    private bool _isTriggered;
    private Vector3 _origin_pos;

    public override void Start()
    {
        base.Start();

        _globalWaypoints = new Vector3[_localWaypoints.Length];
        _origin_pos = transform.position;
        for (int i = 0; i < _localWaypoints.Length; i++)
        {
            _globalWaypoints[i] = _localWaypoints[i] + transform.position;
        }
        _platform.speed = _speed;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void FixedUpdate()
    {
        if(!_isTriggered && IsPlatformReturned()) return; 

        //...........
        UpdateRayOrigins();

        velocity = EvaluatePlatformMovement();
        EvaluatePassangerMovement(velocity);

        MovePassanger(true);
        transform.Translate(velocity);
        MovePassanger(false);

        IsPlatformReturned();
    }

    private float Ease(float t)
    {
        t = Mathf.Clamp01(t);
        return _easing.Evaluate(t);
    }

    Vector3 EvaluatePlatformMovement()
    {
        if (Time.time < _nextMoveTime) {
            boost_buffer -= Time.fixedDeltaTime;
            return Vector3.zero;
        }

        _platform.fromWayPointIndex %= _globalWaypoints.Length;
        int toWaypointIndex = (_platform.fromWayPointIndex + 1) % _globalWaypoints.Length;

        Vector3 startPos = _globalWaypoints[_platform.fromWayPointIndex];
        Vector3 endPos = _globalWaypoints[toWaypointIndex];
        Vector3 moveDirection = (endPos - startPos).normalized;

        float distanceBetweenWaypoints = Vector3.Distance(startPos, endPos);
        _platform.percentageBetweenWaypoints += Time.fixedDeltaTime * _platform.speed / distanceBetweenWaypoints;

        float easedPercent = Ease(_platform.percentageBetweenWaypoints);
        Vector3 _position = Vector3.Lerp(startPos, endPos, easedPercent);

        if (_platform.percentageBetweenWaypoints >= 1)
        {
            _platform.fromWayPointIndex++;
            _platform.percentageBetweenWaypoints = 0;
            _impulseSource.GenerateImpulse(velocity);

            //// Only apply boost/impulse if moving upward
            //if (moveDirection.y > 0)
            //{
            //    if (_impulseSource != null)

            //    boost_buffer = 0.3f;
            //}

            // cyclic handles
            if (_cyclic && _platform.fromWayPointIndex >= _globalWaypoints.Length - 1) {
                _platform.fromWayPointIndex = 0;
                System.Array.Reverse(_globalWaypoints);    
            }

            // non_cyclic handles
            if (_platform.fromWayPointIndex >= _globalWaypoints.Length - 1) {
               this.enabled = false;
            }

            _nextMoveTime = Time.time + _waitTime;
        }

        return _position - transform.position;
    }

    void MovePassanger(bool beforeMovePlatform)
    {

        if (!_passanger.transform) return;
        if (_passanger.moveBeforePlatform == beforeMovePlatform)
        {

            var _player = _passanger.transform.GetComponent<Player>();
            if (_player._stateMachine._currentState._name == PlayerStateList.Jumping)
            {
                OnPlayerJump(_player);
            }
            _passanger.transform.GetComponent<Controller2D>().move(_passanger.velocity, _passanger.onPlatform);
        }
    }
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

                Debug.DrawRay(rayo, Vector3.up * rayLength * directionY, Color.red);
            }
        }

        // horizontal
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < hraycount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? _origins.bottomLeft : _origins.bottomRight;
                rayOrigin += Vector2.up * (hraySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit)
                {
                    float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                    float pushY = -skinWidth;

                    _passanger = new PassangersData(hit.transform, new Vector3(pushX, pushY), false, true);
                }
            }
        }


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
                Debug.DrawRay(rayOrigin, Vector3.up * rayLength, Color.white);
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

    private void OnDrawGizmos()
    {
        if (_localWaypoints == null) return;

        Gizmos.color = Color.yellow;
        var _size = 0.3f;

        for (int i = 0; i < _localWaypoints.Length; i++)
        {
            var globalPos = (Application.isPlaying) ? _globalWaypoints[i] : _localWaypoints[i] + transform.position;
            Gizmos.DrawLine(globalPos - Vector3.up * _size, globalPos + Vector3.up * _size);
            Gizmos.DrawLine(globalPos - Vector3.left * _size, globalPos + Vector3.left * _size);
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player")) {
            _isTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player")) {
            _isTriggered = false;
        }
    }

    public void OnPlayerJump(Player player)
    {
        if (boost_buffer >= 0) {
            player.ApplyPlatformBoost(1, _boost * new Vector3(0, 1, 0));
        }
        else player.ApplyPlatformBoost(_platform.percentageBetweenWaypoints, _boost * velocity);
    }
    private bool IsPlatformReturned() {
        if (_platform.fromWayPointIndex == 0 && Vector3.Distance(transform.position, _origin_pos) <= 0.001f) {
            transform.position = _globalWaypoints[0];
            return true;
        }
        return false;
    }

    public void reset()
    {
        transform.position = _globalWaypoints[0];
        _platform.fromWayPointIndex = 0;
    }
}
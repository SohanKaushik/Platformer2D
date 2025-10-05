using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : PlatformController
{
    private Vector3 _velocity;

    [SerializeField]
    Vector3[] _localWaypoints;
    Vector3[] _globalWaypoints;

    PlatformInformation _platform;

    private HashSet<Controller2D> _passengers = new HashSet<Controller2D>();

    private void Start()
    {
        base.Start();

        _globalWaypoints = new Vector3[_localWaypoints.Length];
        for (int i = 0; i < _localWaypoints.Length; i++) {
            _globalWaypoints[i] = _localWaypoints[i] + transform.position;
        }    
        _original_position = _globalWaypoints[0];
    }
    private void FixedUpdate()
    {
        IsPassengerInTheWay();
       if(!_platform.triggered && IsPlatformReturned()) return;

        _velocity = EvaluatePlatformMovement();
        transform.Translate(_velocity, Space.World);

        MovePassengers(_velocity);
    }

    public override float EvaluatePassengerMovement()
    {
        return 0;
    }

    public override Vector3 EvaluatePlatformMovement()
    {
        _platform.fromWayPointIndex %= _globalWaypoints.Length;
        int toWaypointIndex = (_platform.fromWayPointIndex + 1) % _globalWaypoints.Length;

        float distanceBetweenWaypoints = Vector3.Distance(_globalWaypoints[toWaypointIndex], _globalWaypoints[_platform.fromWayPointIndex]);
        _platform.percentageBetweenWaypoints += Time.fixedDeltaTime * (_speed / distanceBetweenWaypoints);

        float easing = Ease(_platform.percentageBetweenWaypoints);
        
        Vector3 _new_position = Vector3.Lerp(_globalWaypoints[_platform.fromWayPointIndex], _globalWaypoints[toWaypointIndex], easing);

        if (_platform.percentageBetweenWaypoints >= 1) { 
            _platform.percentageBetweenWaypoints = 0;
            _platform.fromWayPointIndex++;

            if (_platform.fromWayPointIndex >= _globalWaypoints.Length - 1) {
                _platform.fromWayPointIndex = 0;

                System.Array.Reverse(_globalWaypoints);
            }
        }

        // # delta movement
        return  _new_position - transform.position;
    }

    private bool IsPlatformReturned()
    {
        if (_platform.fromWayPointIndex == 0 && Vector3.Distance(transform.position, _original_position) <= 0.001f)
        {
            transform.position = _original_position;
            _platform.triggered = false;
            return true;
        }
        return false;
    }

    public override void MovePassengers(Vector3 dv)
    {
        //foreach (var passenger in _passengers)
        //{
        //    passenger.move(dv, true);
        //}
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        _platform.triggered = true;
        if (col.CompareTag("Player")) { 
            _passengers.Add(col.GetComponent<Controller2D>());
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        _platform.triggered = false;
        if (col.CompareTag("Player")) { 
            _passengers.Remove(col.GetComponent<Controller2D>());
        }
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

    struct PlatformInformation
    {
        public bool triggered;
        public int fromWayPointIndex;
        public float percentageBetweenWaypoints;
    }

    private bool IsPassengerInTheWay()
    {
        float directionX = Mathf.Sign(_velocity.x);
        float rayLength = Mathf.Abs(_velocity.x) + skinWidth;


        if (Mathf.Abs(_velocity.x) < skinWidth)
        {
            rayLength = 2f * skinWidth;
        }


        for (int i = 0; i < hraycount; i++)
        {
            var rayo = (directionX == -1) ? _origins.bottomLeft : _origins.bottomRight;
            rayo += Vector2.up * (hraySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.right * directionX, rayLength, passengerMask);

            if (hit)
            {
                var pushX = _velocity.x - (hit.distance - skinWidth) * directionX;
               // hit.transform.GetComponent<Controller2D>().move(_velocity);
                return true;
            }

            Debug.DrawRay(rayo, Vector2.right * directionX * rayLength, Color.red);
        }
        return false;
    }

    public Vector3 GetVelocity() => _velocity;
    //private bool IsPassengerOnPlatform(Controller2D passenger)
    //{
    //    // Check if passenger is actually on top of the platform
    //    return Physics2D.OverlapBox(transform.position, _collider.bounds.size, 0, passengerMask) != null;
    //}
}

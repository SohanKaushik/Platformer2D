using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PlatformController : RaycastController {

    [SerializeField] private Vector2 _detectionRange;
    //[SerializeField] private Vector2 _detectionOffset;

    [SerializeField] LayerMask _layermask;

   public Vector3 _velocity = Vector3.down;
   

    private bool _detected;

    protected virtual void FixedUpdate() {

        UpdateRayOrigins();
        VerticalCollision(ref _velocity);
    }

    protected override void HorizontalCollision(ref Vector3 velocity)
    {
        float directionX = _colldata.direction;

        for (int i = 0; i < hraycount; i++)
        {
            Vector2 rayo = (directionX == -1) ? _origins.bottomLeft : _origins.bottomRight;
            rayo += Vector2.up * (hraySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.right * directionX, _detectionRange.x, _layermask);


            if (hit)
            {
                Debug.Log(hit.transform.gameObject.name);
            }
            Debug.DrawRay(rayo, Vector2.up * directionX * _detectionRange.x, Color.blue);
        }
    }

    protected override void VerticalCollision(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);

        for (int i = 0; i < hraycount; i++)
        {
            Vector2 rayo = (directionY == -1) ? _origins.bottomLeft : _origins.topLeft;
            rayo += Vector2.right * (vraySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayo, Vector2.up * directionY, _detectionRange.y, _layermask);


            if (hit)
            {
                _detected = true;
            }
            Debug.DrawRay(rayo, Vector2.up * directionY * _detectionRange.y, Color.red);
        }
    }

    public bool isDetected() => _detected;
}

using UnityEngine;
using UnityEngine.Rendering;

public class test_player : Controller2D
{
    public MovingPlatforms _currentPlatform;

    private void Start()
    {
        base.Start();
    }

    void Update()
    {
        var input = Input.GetAxisRaw("Horizontal");
        
       if (_currentPlatform != null && isGrounded())
       {
           transform.position += (Vector3)_currentPlatform.GetDeltaMovement() + new Vector3(input, 0, 0) * Time.deltaTime;
       }
        UpdateRayOrigins();
    }

    bool isGrounded()
    {
        Debug.Log(01010);
        float rayLength = 0.1f + skinWidth;

        for (int i = 0; i < hraycount; i++)
        {
            Vector2 delta = _currentPlatform != null ? _currentPlatform.GetDeltaMovement() : Vector2.zero;
            Vector2 rayOrigin = (_origins.bottomLeft + delta) + Vector2.right * (hraySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, LayerMask.GetMask("Platforms"));

            Debug.DrawRay(rayOrigin, Vector2.down , Color.green);
            if (hit) return true;
        }
        return false;

    }
}

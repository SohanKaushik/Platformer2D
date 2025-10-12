using UnityEngine;
using UnityEngine.Rendering;

public class test_player : Controller2D
{
    public MovingPlatforms _currentPlatform;
    public float speed;

    private void Start()
    {
        base.Start();
    }

    void Update()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
       if (_currentPlatform != null && isGrounded())
       {
           transform.position += (Vector3)_currentPlatform.GetDeltaMovement();
       }

        transform.Translate(input * speed * Time.deltaTime);
        UpdateRayOrigins();
    }

    bool isGrounded()
    {
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

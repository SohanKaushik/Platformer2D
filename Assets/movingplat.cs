using UnityEngine;

public class movingplat : MonoBehaviour
{
    public float speed = 2f;
    public Vector2 move = Vector2.right;

    private Vector2 lastPosition;
    private Vector2 velocity;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Move the platform
        transform.Translate(move * speed * Time.deltaTime);

        // Calculate velocity based on actual movement
        Vector2 currentPosition = transform.position;
        velocity = currentPosition - lastPosition;
        lastPosition = currentPosition;
    }

    public Vector2 GetDeltaMovement()
    {
        return velocity;
    }
}

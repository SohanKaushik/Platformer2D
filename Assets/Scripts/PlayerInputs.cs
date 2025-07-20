using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    private Vector2 _input;
    public void _requests(ref bool action, bool input) {
        if (input) action = true;
    }
    public Vector2 _inputs() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}

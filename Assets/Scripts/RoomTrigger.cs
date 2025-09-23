using Unity.Cinemachine;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            _camera.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            _camera.SetActive(false);
        }
    }
}

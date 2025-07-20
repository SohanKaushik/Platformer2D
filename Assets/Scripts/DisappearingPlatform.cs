using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [SerializeField] private float disappearDelay = 0.5f;
    [SerializeField] private float reappearDelay = 2f;
    [SerializeField] private bool shouldReappear = true;

    [SerializeField] GameObject _platform;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            Invoke(nameof(Disappear), disappearDelay);
        }
    }

    private void Disappear()
    {
        _platform.GetComponent<Collider2D>().enabled = false;
        _platform.GetComponent<MeshRenderer>().enabled = false;

        if (shouldReappear)
            Invoke(nameof(Reappear), reappearDelay);
    }

    private void Reappear()
    {
        _platform.GetComponent<Collider2D>().enabled = true;
        _platform.GetComponent<MeshRenderer>().enabled = true;
    }
}

using System.Collections;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    private Vector3 _originalPosition;
    [SerializeField] Vector3 _targetPosition;

    [SerializeField] float _duration = 1.0f;

    private void Start()
    {
        _originalPosition = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            StartCoroutine(move(_targetPosition * Time.fixedDeltaTime));
        }
    }

    private IEnumerator move(Vector3 target)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _duration;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = _targetPosition;
    }
}

using System.Collections;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] float _force = 20f;
    [SerializeField] float _squashDistance = 0.5f;
    [SerializeField] float _squashDuration = 0.1f;

    private Vector3 _originalPosition;
    private Coroutine _squashRoutine;

    private void Start()
    {
        _originalPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<Player>();
            player._velocity.y = _force;
            player._isDashing = false;

            transform.position = _originalPosition;
            if (_squashRoutine != null) StopCoroutine(_squashRoutine);
            _squashRoutine = StartCoroutine(SquashTrampoline(_originalPosition + Vector3.up * _squashDistance));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_squashRoutine != null) StopCoroutine(_squashRoutine);
            _squashRoutine = StartCoroutine(SquashTrampoline(_originalPosition));
        }
    }

    private IEnumerator SquashTrampoline(Vector3 targetPos)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        
        while (elapsed < _squashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _squashDuration;
            transform.position = Vector3.Lerp(start, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
    }
}

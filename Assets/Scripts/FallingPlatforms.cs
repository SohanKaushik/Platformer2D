using Unity.Cinemachine;
using UnityEngine;

public class FallingPlatforms : PlatformController {

    private CinemachineImpulseSource _impulse;
    private float _timer;

    [SerializeField] float _duration = 1f;
    [SerializeField] AnimationCurve _curve;

    private Vector3 _startPosition;
    private bool _completed = false;

    private void Awake()
    {
        _startPosition = transform.position;
        _impulse = GetComponent<CinemachineImpulseSource>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!isDetected() || _completed) return;

        _timer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(_timer / _duration);
        float evaluated = _curve.Evaluate(t);

        transform.position = _startPosition + new Vector3(0, evaluated * _velocity.y, 0);

        if (t >= 1 && !_completed) {
            _impulse.GenerateImpulse(Vector3.down);
            _completed = true;
        }
    }
}

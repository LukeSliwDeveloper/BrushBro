using UnityEngine;
using UnityEngine.InputSystem;

public class Brush : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private LineRenderer _lineRenderer;
    
    private Vector2 _mousePosition;
    private bool _mousePositionFound;
    private float _mousePositionX;
    private float _targetPositionX;

    private void Awake()
    {
        _moveSpeed *= Time.fixedDeltaTime;
    }

    private void OnLook(InputValue value)
    {
        _mousePosition = value.Get<Vector2>();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(value.Get<Vector2>()),out var hit, Mathf.Infinity))
        {
            _mousePositionX = hit.point.x;
            _mousePositionFound = true;
        }
        else
            _mousePositionFound = false;
    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
        if (_mousePositionFound)
        {
            _targetPositionX = _rigidbody.position.x + Mathf.Clamp(_mousePositionX - _rigidbody.position.x, -_moveSpeed, _moveSpeed);
            _rigidbody.MovePosition(new Vector3(_targetPositionX, 0f, 0f));
        }

        for (int i = _lineRenderer.positionCount - 1; i > 0; i--)
        {
            _lineRenderer.SetPosition(i, new Vector3(_lineRenderer.GetPosition(i - 1).x, -.4999f, i * .1f));
        }
        _lineRenderer.SetPosition(0, new Vector3(_targetPositionX, -.4999f, 0f));
        _lineRenderer.SetPosition(1, Vector3.Lerp(_lineRenderer.GetPosition(1), (_lineRenderer.GetPosition(0) + _lineRenderer.GetPosition(2))/2f, .76f));
        float averageLineAngle = 0f;
        for (int i = 0; i < 5; i++)
        {
            averageLineAngle += Vector3.SignedAngle(_lineRenderer.GetPosition(i) - _lineRenderer.GetPosition(i+1), Vector3.back, Vector3.down);
        }
        averageLineAngle /= 5f;
        _modelTransform.rotation = Quaternion.Euler(-25f, averageLineAngle, 0f);
    }
}

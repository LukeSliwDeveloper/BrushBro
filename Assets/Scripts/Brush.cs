using UnityEngine;
using UnityEngine.InputSystem;

public class Brush : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _modelTransform;
    
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
        if (GameManager.Instance.CurrentGameState == GameState.Gameplay)
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
    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
        if (_mousePositionFound)
        {
            _targetPositionX = _rigidbody.position.x + Mathf.Clamp(_mousePositionX - _rigidbody.position.x, -_moveSpeed, _moveSpeed);
            _rigidbody.MovePosition(new Vector3(_targetPositionX, 0f, 0f));
        }

        _modelTransform.rotation = Quaternion.Euler(-25f, 0f, 0f);
    }
}

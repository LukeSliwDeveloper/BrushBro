using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Brush : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Vector3 _hitKnockback = Vector3.forward * .2f;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;
    [SerializeField] private Transform _modelTransform;

    private int _roadLayerMask = 1 << 3;
    private int _flickerTicksAmount = 13;
    private WaitForSeconds _flickerTickDuration = new WaitForSeconds(0.15f);

    private Vector3 _pushingForce;
    private Vector3 _targetPosition;
    private Vector3 _mousePosition;
    private bool _mousePositionFound;
    private bool _movedThisTick;
    private bool _isImmune = false;
    private bool _isInert = false;

    private void OnLook(InputValue value)
    {
        if (GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(value.Get<Vector2>()), out var hit, Mathf.Infinity, _roadLayerMask))
            {
                _mousePosition = hit.point;
                _mousePosition.y = 0f;
                _mousePositionFound = true;
            }
            else
                _mousePositionFound = false;
        }
        else
            _mousePositionFound = false;
    }

    private void FixedUpdate()
    {
        if (!_isInert && _mousePositionFound)
        {
            _targetPosition = _rigidbody.position + Vector3.ClampMagnitude(_mousePosition - _rigidbody.position, _moveSpeed * Time.fixedDeltaTime);
            if (_targetPosition.z > 4.2f || _targetPosition.z < -3.5f)
                _targetPosition.z = Mathf.Clamp(_targetPosition.z, -3.5f, 4.2f);
            if (_targetPosition.x > 4f || _targetPosition.x < -4f)
            {
                _targetPosition.x = Mathf.Clamp(_targetPosition.x, -4f, 4f);
                TakeDamage();
            }
            if (_pushingForce != Vector3.zero)
            {
                _targetPosition += _pushingForce;
                _pushingForce = Vector3.zero;
            }
            _movedThisTick = _targetPosition != _rigidbody.position;
            _rigidbody.MovePosition(_targetPosition);
        }
        _modelTransform.rotation = Quaternion.Euler(-25f, 0f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bump"))
            TakeDamage();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bump") && _movedThisTick)
            TakeDamage();
    }

    public void TakeDamage()
    {
        if (!_isImmune)
        {
            if (GameplayManager.Instance.RevivePlayer())
            {
                _isInert = _isImmune = true;
                _pushingForce = _hitKnockback;
                StartCoroutine(Flicker());
            }
            else
                _collider.enabled = false;
        }
    }

    private IEnumerator Flicker()
    {
        for (int i = 0; i < _flickerTicksAmount; i++)
        {
            if (i == 2)
                _isInert = false;
            _modelTransform.gameObject.SetActive(!_modelTransform.gameObject.activeSelf);
            yield return _flickerTickDuration;
        }
        _modelTransform.gameObject.SetActive(true);
        _isImmune = false;
    }
}

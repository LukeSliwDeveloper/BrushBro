using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Brush : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Vector3 _hitKnockback = Vector3.forward * .2f;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;
    [SerializeField] private AudioSource _hurtAudio;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private LineRenderer _lineRenderer;

    private int _roadLayerMask = 1 << 3;
    private int _flickerTicksAmount = 13;
    private float _updatePathDelay = .05f;
    private WaitForSeconds _flickerTickDuration = new WaitForSeconds(0.15f);

    List<Vector3> _linePositions = new List<Vector3>();
    private Vector3 _pushingForce;
    private Vector3 _targetPosition;
    private Vector3 _mousePosition;
    private bool _mousePositionFound;
    private bool _movedThisTick;
    private bool _isImmune = false;
    private bool _isInert = false;
    private GameManager _gameManager;

    private void GameManager_OnLooked(Vector2 value)
    {
        if (GameManager.Instance.CurrentGameState == GameState.Gameplay)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(value), out var hit, Mathf.Infinity, _roadLayerMask))
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

    private void Awake()
    {
        _linePositions.Add(_lineRenderer.GetPosition(0));
        _gameManager = GameManager.Instance;
        _gameManager.OnLooked += GameManager_OnLooked;
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
            _gameManager.OnLooked -= GameManager_OnLooked;
    }

    private void Update()
    {
        if (!_isInert && _mousePositionFound)
        {
            _targetPosition = transform.position + Vector3.ClampMagnitude(_mousePosition - transform.position, _moveSpeed * Time.deltaTime);
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
            _movedThisTick = _targetPosition != transform.position;
            float averageLineAngle = 0f;
            int positionsCount = Mathf.Min(15, _linePositions.Count);
            Vector3 newLine;
            for (int i = 1; i <= positionsCount; i++)
            {
                newLine = _linePositions[_linePositions.Count - i]; newLine.y = 0f;
                averageLineAngle += Vector3.SignedAngle(transform.position - newLine, Vector3.back, Vector3.down);
            }
            averageLineAngle /= positionsCount;
            _modelTransform.rotation = Quaternion.Euler(-25f, Mathf.Clamp(averageLineAngle, -15f, 15f), 0f);
            transform.position = _targetPosition;
            if ((_updatePathDelay -= Time.deltaTime) <= 0f)
            {
                _updatePathDelay += .05f;
                Vector3 upVector = new(0f, 0f, GameplayManager.Instance.RoadSpeed * (0.05f + _updatePathDelay));
                for (int i = 0; i < _linePositions.Count;)
                {
                    if ((_linePositions[i] += upVector).z >= 4.8f)
                        _linePositions.RemoveAt(i);
                    else
                        i++;
                }
                _linePositions.Add(transform.position + Vector3.down * .49f);
                _lineRenderer.positionCount = _linePositions.Count;
                _lineRenderer.SetPositions(_linePositions.ToArray());
            }
        }
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
                _hurtAudio.Play();
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

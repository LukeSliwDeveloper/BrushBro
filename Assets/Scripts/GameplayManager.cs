using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] private Transform _roadTransform;

    private float _speed = 2f;
    private bool _isLevelRunning;

    private void FixedUpdate()
    {
        _roadTransform.position += Vector3.forward * Time.fixedDeltaTime * _speed;
        if (_roadTransform.position.z > 18f)
            _roadTransform.position -= new Vector3(0f, 0f, 18f);
    }

    public void StartGame() => _isLevelRunning = true;
}

using UnityEngine;

public class RoadObject : MonoBehaviour
{
    [SerializeField] private Vector3 _additionalSpeed = Vector3.zero;
    [SerializeField] private bool _destoryOnCollision = false;

    private GameplayManager _gameplayManager;

    private void OnEnable()
    {
        _gameplayManager = GameplayManager.Instance;
        _gameplayManager.OnMove += GameplayManager_OnMove;
    }

    private void OnDisable()
    {
        if (_gameplayManager != null)
            _gameplayManager.OnMove -= GameplayManager_OnMove;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_destoryOnCollision && collision.CompareTag("Respawn"))
            Destroy(gameObject);
    }

    private void GameplayManager_OnMove()
    {
        transform.position = transform.position + (Vector3.forward * GameplayManager.Instance.RoadSpeed + _additionalSpeed) * Time.deltaTime;
        if (transform.position.z >= 7f)
            Destroy(gameObject);
    }
}

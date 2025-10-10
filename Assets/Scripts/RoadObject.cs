using UnityEngine;

public class RoadObject : MonoBehaviour
{
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

    private void GameplayManager_OnMove()
    {
        transform.position = transform.position + Vector3.forward * Time.deltaTime * GameplayManager.Instance.RoadSpeed;
        if (transform.position.z >= 7f)
            Destroy(gameObject);
    }
}

using System;
using UnityEngine;

public class Cursoe : MonoBehaviour
{
    private GameManager _gameManager;

    private void Awake()
    {
        Cursor.visible = false;
        _gameManager = GameManager.Instance;
        _gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        _gameManager.OnLooked += GameManager_OnLooked;
    }

    private void GameManager_OnLooked(Vector2 vector)
    {
        transform.position = vector;
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
        {
            _gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
            _gameManager.OnLooked -= GameManager_OnLooked;
        }
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        if (state == GameState.Gameplay)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}

using System;
using TMPro;
using UnityEngine;

public class GameOverView : UserInterfaceView
{
    [SerializeField] private TMP_Text _bestScoreText, _scoreText;

    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
            _gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
        {
            _gameManager.SetTopScore(_userInterface.Score);
            _bestScoreText.text = _gameManager.TopScore.ToString();
            _scoreText.text = _userInterface.Score.ToString();
            Debug.Log("New Top Score: " + _gameManager.TopScore);
            gameObject.SetActive(true);
        }
    }
}

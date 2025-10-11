using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private Sprite[] _livesLeftSprites;
    [SerializeField] private Sprite _highlightedSprite;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Image _titleImage;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private GameObject _rmbTutorialGO, _bumpTutorialGO;

    private GameManager _gameManager;
    private GameplayManager _gameplayManager;
    private bool _isHighlighted = false;

    public float Score { get; private set; }

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        _gameplayManager = GameplayManager.Instance;
        _gameplayManager.OnMove += GameplayManager_OnMove;
        _gameplayManager.OnPlayerHit += GameplayManager_OnPlayerHit;
        _gameplayManager.OnBumpTutorialComplete += GameplayManager_OnBumpTutorialComplete;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
            _gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        if (_gameplayManager != null)
        {
            _gameplayManager.OnMove -= GameplayManager_OnMove;
            _gameplayManager.OnPlayerHit -= GameplayManager_OnPlayerHit;
            _gameplayManager.OnBumpTutorialComplete -= GameplayManager_OnBumpTutorialComplete;
        }
    }

    private void GameplayManager_OnBumpTutorialComplete(bool complete)
    {
        if (complete)
            _bumpTutorialGO.SetActive(false);
        else
            _bumpTutorialGO.SetActive(true);
    }

    private void GameplayManager_OnPlayerHit(int lives)
    {
        _livesImage.sprite = _livesLeftSprites[lives < 0 ? 0 : lives];
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {
        if (state == GameState.Gameplay)
        {
            _rmbTutorialGO.SetActive(!_gameManager.WasRmbTutorialComplete);
            gameObject.SetActive(true);
        }
        else
        {
            if (state == GameState.Pause)
                _rmbTutorialGO.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void GameplayManager_OnMove()
    {
        Score += 2 * Time.deltaTime;
        if (!_isHighlighted && (Mathf.RoundToInt(Score) > _gameManager.TopScore))
        {
            _titleImage.sprite = _highlightedSprite;
            _isHighlighted = true;
        }
        _text.text = Mathf.RoundToInt(Score).ToString();
    }
}

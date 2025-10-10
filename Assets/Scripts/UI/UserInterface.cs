using UnityEngine;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private AudioSource _clickSfx, _hoverSfx;
    [SerializeField] private ScoreView _scoreView;
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _settingsGO;

    GameManager _gameManager;

    public int Score => Mathf.RoundToInt(_scoreView.Score);

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _gameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
            _gameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    public void PlayClick() => _clickSfx.Play();

    public void PlayHover() => _hoverSfx.Play();

    public void OpenSettings()
    {
        _settingsGO.SetActive(true);
    }

    public void OpenMenu()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Menu)
            _mainMenuGO.SetActive(true);
    }

    public void ReloadScene() => GameManager.Instance.ReloadScene();

    public void ResetSave() => GameManager.Instance.ResetSave();

    private void GameManager_OnGameStateChanged(GameState state)
    {
        if (state == GameState.Pause)
            _settingsGO.SetActive(true);
    }
}

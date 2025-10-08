using UnityEngine;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuGO;
    [SerializeField] private GameObject _settingsGO;

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
}

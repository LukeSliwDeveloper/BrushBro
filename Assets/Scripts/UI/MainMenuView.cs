using UnityEngine;

public class MainMenuView : UserInterfaceView
{
    public void StartGameplay()
    {
        _userInterface.PlayClick();
        gameObject.SetActive(false);
        GameManager.Instance.ChangeGameState(GameState.Gameplay);
    }

    public void OpenSettings()
    {
        _userInterface.PlayClick();
        gameObject.SetActive(false);
        _userInterface.OpenSettings();
    }

    public void OpenShop()
    {

    }
}

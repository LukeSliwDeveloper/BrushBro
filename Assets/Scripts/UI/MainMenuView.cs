using UnityEngine;

public class MainMenuView : UserInterfaceView
{
    public void StartGameplay()
    {
        gameObject.SetActive(false);
        GameManager.Instance.ChangeGameState(GameState.Gameplay);
    }

    public void OpenSettings()
    {
        gameObject.SetActive(false);
        _userInterface.OpenSettings();
    }

    public void OpenShop()
    {

    }
}

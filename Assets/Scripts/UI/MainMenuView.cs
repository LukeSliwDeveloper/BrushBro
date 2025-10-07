using UnityEngine;

public class MainMenuView : UserInterfaceView
{
    public void StartGameplay()
    {

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

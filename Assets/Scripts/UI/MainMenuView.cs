using UnityEngine;

public class MainMenuView : MonoBehaviour
{
    [SerializeField] private UserInterface _userInterface;

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

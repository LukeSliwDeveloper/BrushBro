using UnityEngine;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private GameObject _settingsGO;

    public void OpenSettings()
    {
        _settingsGO.SetActive(true);
    }
}

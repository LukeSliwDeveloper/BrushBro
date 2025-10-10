using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : UserInterfaceView
{
    [SerializeField] private Sprite OcclusionOnSprite, OcclusionOffSprite, OcclusionOnIdleSprite, OcclusionOffIdleSprite;
    [SerializeField] private Slider _masterSlider, _musicSlider, _sfxSlider;
    [SerializeField] private Image _occlusionImage;
    [SerializeField] private Button _occlusionButton;

    private void Awake()
    {
        _masterSlider.value = GameManager.Instance.MasterVolume;
        _musicSlider.value = GameManager.Instance.MusicVolume;
        _sfxSlider.value = GameManager.Instance.SfxVolume;
        AdjustOcclusionSprites();
    }

    public void CloseSettings()
    {
        _userInterface.PlayClick();
        gameObject.SetActive(false);
        if (GameManager.Instance.CurrentGameState == GameState.Pause)
            GameManager.Instance.Unpause();
        else
            _userInterface.OpenMenu();
    }

    public void ResetMasterVolume()
    {
        _userInterface.PlayClick();
        GameManager.Instance.SetMasterVolume(_masterSlider.value = .5f);
    }

    public void ResetMusicVolume()
    {
        _userInterface.PlayClick();
        GameManager.Instance.SetMusicVolume(_musicSlider.value = .5f);
    }

    public void ResetSfxVolume()
    {
        _userInterface.PlayClick();
        GameManager.Instance.SetSfxVolume(_sfxSlider.value = 1f);
    }

    public void ChangeMasterVolume(float value) => GameManager.Instance.SetMasterVolume(value);

    public void ChangeMusicVolume(float value) => GameManager.Instance.SetMusicVolume(value);

    public void ChangeSfxVolume(float value) => GameManager.Instance.SetSfxVolume(value);

    public void ChangeOcclusion()
    {
        _userInterface.PlayClick();
        GameManager.Instance.SetOcclusionEnabled(!GameManager.Instance.OcclusionEnabled);
        AdjustOcclusionSprites();
    }

    private void AdjustOcclusionSprites()
    {
        if (GameManager.Instance.OcclusionEnabled)
        {
            _occlusionImage.sprite = OcclusionOnIdleSprite;
            _occlusionButton.spriteState = new SpriteState
            {
                highlightedSprite = OcclusionOnSprite,
            };
        }
        else
        {
            _occlusionImage.sprite = OcclusionOffIdleSprite;
            _occlusionButton.spriteState = new SpriteState
            {
                highlightedSprite = OcclusionOffSprite,
            };
        }
    }
}

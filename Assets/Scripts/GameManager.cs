using DG.Tweening;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private ScriptableRendererData _rendererData;
    [SerializeField] private Volume _volume;

    public float MasterVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;
    public float SfxVolume { get; private set; } = 1f;
    public bool OcclusionEnabled { get; private set; } = true;
    public int TopScore { get; private set; } = 0;
    public GameState CurrentGameState { get; set; } = GameState.Menu;

    public event Action<GameState> OnGameStateChanged;

    protected override bool Awake()
    {
        if (base.Awake())
        {
            DontDestroyOnLoad(gameObject);
            return true;
        }
        return false;
    }

    private void Start()
    {
        LoadSave();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
        ChangeGameState(GameState.Menu);
    }

    public void ChangeGameState(GameState gameState)
    {
        if (_volume.profile.TryGet(out DepthOfField dof))
        {
            var focalLength = (gameState == GameState.Gameplay ? 1f : 300f);
            DOTween.To(x => dof.focalLength.value = x, dof.focalLength.value, focalLength, .3f);
        }
        CurrentGameState = gameState;
        OnGameStateChanged?.Invoke(CurrentGameState);
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.Save();
        ApplyMasterVolume();
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.Save();
        ApplyMusicVolume();
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = value;
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
        PlayerPrefs.Save();
        ApplySfxVolume();
    }

    public void SetOcclusionEnabled(bool enabled)
    {
        OcclusionEnabled = enabled;
        PlayerPrefs.SetInt("OcclusionEnabled", OcclusionEnabled ? 1 : 0);
        PlayerPrefs.Save();
        ApplyOcclusion();
    }

    public void SetTopScore(int score)
    {
        if (score > TopScore)
        {
            TopScore = score;
            PlayerPrefs.SetInt("TopScore", TopScore);
            PlayerPrefs.Save();
        }
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        LoadSave();
    }

    private void LoadSave()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        ApplyMasterVolume();
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        ApplyMusicVolume();
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
        ApplySfxVolume();
        OcclusionEnabled = PlayerPrefs.GetInt("OcclusionEnabled", 1) == 1;
        ApplyOcclusion();
        TopScore = PlayerPrefs.GetInt("TopScore", 0);
    }

    private void ApplyMasterVolume() => _audioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20f);

    private void ApplyMusicVolume() => _audioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicVolume) * 20f);

    private void ApplySfxVolume() => _audioMixer.SetFloat("SfxVolume", Mathf.Log10(SfxVolume) * 20f);

    private void ApplyOcclusion()
    {
        _rendererData.rendererFeatures[0].SetActive(OcclusionEnabled);
        var mi = _rendererData.GetType().GetMethod("SetDirty", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        mi?.Invoke(_rendererData, null);
    }
}

public enum GameState
{
    Menu,
    Gameplay,
    GameOver
}
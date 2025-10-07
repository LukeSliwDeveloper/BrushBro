using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private AudioMixer audioMixer;

    public float MasterVolume { get; private set; } = 1f;
    public float MusicVolume { get; private set; } = 1f;
    public float SfxVolume { get; private set; } = 1f;
    public bool OcclusionEnabled { get; private set; } = true;
    public int TopScore { get; private set; } = 0;

    protected override bool Awake()
    {
        if (base.Awake())
        {
            LoadSave();
        }
        return true;
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
        TopScore = PlayerPrefs.GetInt("TopScore", 0);
    }

    private void ApplyMasterVolume() => audioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20f);

    private void ApplyMusicVolume() => audioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicVolume) * 20f);

    private void ApplySfxVolume() => audioMixer.SetFloat("SfxVolume", Mathf.Log10(SfxVolume) * 20f);
}

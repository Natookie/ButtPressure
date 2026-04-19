using UnityEngine;
using NovaSamples.UIControls;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Component and Object")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle musicMuteToggle;
    [SerializeField] private Toggle sfxMuteToggle;

    [Header("Initial Settings")]
    [SerializeField] private float musicVolume = 1.0f;
    [SerializeField] private float sfxVolume = 1.0f;
    [SerializeField] private bool isMusicMute = false;
    [SerializeField] private bool isSfxMute = false;

    // ====================================================================================================
    //                     Virtual Methods
    // ====================================================================================================
    #region Virtual
    private void Start()
    {
        // Connect UI events
        if (musicVolumeSlider) musicVolumeSlider.OnValueChanged.AddListener(
            value =>
            {
                musicVolume = value;
                SaveSettings();
            }
        );
        if (sfxVolumeSlider) sfxVolumeSlider.OnValueChanged.AddListener(
            value =>
            {
                sfxVolume = value;
                SaveSettings();
            }
        );
        if (musicMuteToggle) musicMuteToggle.OnToggled.AddListener(
            value =>
            {
                isMusicMute = value;
                SaveSettings();
            }
        );
        if (sfxMuteToggle) sfxMuteToggle.OnToggled.AddListener(
            value =>
            {
                isSfxMute = value;
                SaveSettings();
            }
        );
        // Initialize
        LoadSettings();
    }
    #endregion

    // ====================================================================================================
    //                     Settings Methods
    // ====================================================================================================
    #region Settings
    private void ApplySettings()
    {
        // Check audio manager
        if (!AudioManager.Instance) return;
        // Set volume
        AudioManager.Instance.SetMusicVolume(musicVolume);
        AudioManager.Instance.SetSFXVolume(sfxVolume);
        // Set mute
        AudioManager.Instance.SetMusicMute(isMusicMute);
        AudioManager.Instance.SetSFXMute(isSfxMute);
    }

    public void SaveSettings()
    {
        // Save settings
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("EffectsVolume", sfxVolume);
        PlayerPrefs.SetInt("MuteMusic", isMusicMute ? 1 : 0);
        PlayerPrefs.SetInt("MuteEffects", isSfxMute ? 1 : 0);
        PlayerPrefs.Save();
        // Apply new saved changes
        ApplySettings();
    }

    public void LoadSettings()
    {
        // Load settings
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        isMusicMute = PlayerPrefs.GetInt("MusicMute", 0) == 1;
        isSfxMute = PlayerPrefs.GetInt("SFXMute", 0) == 1;
        // Apply settings
        ApplySettings();
        // Set UI values
        if (musicVolumeSlider) musicVolumeSlider.Value = musicVolume;
        if (sfxVolumeSlider) sfxVolumeSlider.Value = sfxVolume;
        if (musicMuteToggle) musicMuteToggle.ToggledOn = isMusicMute;
        if (sfxMuteToggle) sfxMuteToggle.ToggledOn = isSfxMute;
    }
    #endregion
}

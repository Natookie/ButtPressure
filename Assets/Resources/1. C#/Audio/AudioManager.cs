using System;
// using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Serializable]
    public struct AudioData
    {
        public string name;
        public AudioClip audioClip;
        [Range(0.0f, 1.0f)] public float volume;
        [Range(-3.0f, 3.0f)] public float pitch;
    }

    public static AudioManager Instance {private set; get;}

    [Header("Component and Object")]
    [Tooltip("Audio source for music")]
    [SerializeField] private AudioSource musicSource;
    [Tooltip("Audio source for SFX")]
    [SerializeField] private AudioSource sfxSource;
    [Tooltip("Audio source for SFX looping")]
    [SerializeField] private AudioSource sfxLoopingSource;
    // ====================================================================================================
    [Header("Audio Mixer")]
    [Tooltip("Target audio mixer for audio setings")]
    [SerializeField] private AudioMixer audioMixer;
    [Tooltip("Music volume exposed parameter name in the audio mixer")]
    [SerializeField] private string musicVolumeParameterName = "MusicVolume";
    [Tooltip("SFX volume exposed parameter name in the audio mixer")]
    [SerializeField] private string sfxVolumeParameterName = "SFXVolume";
    // ====================================================================================================
    [Header("Audio List")]
    [Tooltip("List of audio data for music, audio assets need to be listed here before being used")]
    [SerializeField] private AudioData[] musicAudioDataList;
    [Tooltip("List of audio data for SFX, audio assets need to be listed here before being used")]
    [SerializeField] private AudioData[] sfxAudioDataList;

    private float musicVolume = 1.0f;
    private float sfxVolume = 1.0f;
    // private Tween musicFadeTween;
    private string currentSFXLoopingName;

    // ====================================================================================================
    //                     Virtual Methods
    // ====================================================================================================
    #region Virtual
    private void Awake()
    {
        // if (Instance) Destroy(gameObject);
        // else
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        Instance = this;
    }

    private void Start()
    {
        // Assertion Check
        Debug.Assert(musicSource, "musicSource is missing");
        Debug.Assert(sfxSource, "sfxSource is missing");
        Debug.Assert(sfxLoopingSource, "sfxLoopingSource is missing");
        Debug.Assert(audioMixer, "audioMixer is empty");
        // Initialize
        if (sfxLoopingSource) sfxLoopingSource.loop = true;
    }
    #endregion

    // ====================================================================================================
    //                     Music Methods
    // ====================================================================================================
    #region Music
    public void PlayMusic(string audioName, bool isLoop = true)
    {
        // Check music source
        if (!musicSource) return;
        // Find the audio data
        AudioData audioData = Array.Find(musicAudioDataList, audioData => audioData.name == audioName);
        // Check if data was found
        if (audioData.IsUnityNull()) Debug.Log($"Music of: {audioName}, was not found");
        // Play audio
        else
        {
            musicSource.loop = isLoop;
            musicSource.clip = audioData.audioClip;
            musicSource.volume = audioData.volume;
            musicSource.pitch = audioData.pitch;
            musicSource.Play();
        }
    }
    // public void PlayMusic(string audioName, float fadeDuration, bool isLoop = true)
    // {
    //     // Check music source
    //     if (!musicSource) return;
    //     // Find the audio data
    //     AudioData audioData = Array.Find(musicAudioDataList, audioData => audioData.name == audioName);
    //     // Check if data was found
    //     if (audioData.IsUnityNull()) Debug.Log($"Music of: {audioName}, was not found");
    //     else
    //     {
    //         // Play audio
    //         musicSource.loop = isLoop;
    //         musicSource.clip = audioData.audioClip;
    //         musicSource.pitch = audioData.pitch;
    //         musicSource.Play();
    //         // Set fade
    //         musicSource.volume = 0.0f;
    //         if (!musicFadeTween.IsUnityNull()) musicFadeTween.Kill();
    //         musicFadeTween = DOTween.To(
    //             () => {return musicSource.volume;},
    //             (float value) => {musicSource.volume = value;},
    //             audioData.volume,
    //             fadeDuration
    //         );
    //     }
    // }

    public void StopMusic() {musicSource.Stop();}
    // public void StopMusic(float fadeDuration)
    // {
    //     // Set fade
    //     if (!musicFadeTween.IsUnityNull()) musicFadeTween.Kill();
    //     musicFadeTween = DOTween.To(
    //         () => {return musicSource.volume;},
    //         (float value) => {musicSource.volume = value;},
    //         0.0f,
    //         fadeDuration
    //     );
    // }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        audioMixer.SetFloat(musicVolumeParameterName, LinearToDecibles(volume));
    }

    public void SetMusicMute(bool isMute) {musicSource.mute = isMute;}
    #endregion

    // ====================================================================================================
    //                     SFX Methods
    // ====================================================================================================
    #region SFX
    public void PlaySFX(string audioName)
    {
        // Check SFX source
        if (!sfxSource) return;
        // Find the audio data
        AudioData audioData = Array.Find(sfxAudioDataList, audioData => audioData.name == audioName);
        // Check if data was found
        if (audioData.IsUnityNull()) Debug.Log($"SFX of: {audioName}, was not found");
        // Play audio
        else sfxSource.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    public void PlaySFXLooping(string audioName, bool isOverride = false)
    {
        // Check SFX source
        if (!sfxSource) return;
        // Check for override
        if (audioName == currentSFXLoopingName && !isOverride) return;
        // Find the audio data
        AudioData audioData = Array.Find(sfxAudioDataList, audioData => audioData.name == audioName);
        // Check if data was found
        if (audioData.IsUnityNull()) Debug.Log($"SFX of: {audioName}, was not found");
        // Play audio
        else
        {
            sfxLoopingSource.loop = true;
            sfxLoopingSource.clip = audioData.audioClip;
            sfxLoopingSource.volume = audioData.volume;
            sfxLoopingSource.pitch = audioData.pitch;
            sfxLoopingSource.Play();
            currentSFXLoopingName = audioName;
        }
    }

    public void StopSFXAll()
    {
        sfxSource.Stop();
        StopSFXLooping();
    }

    public void StopSFXLooping()
    {
        sfxLoopingSource.Stop();
        currentSFXLoopingName = "";
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer.SetFloat(sfxVolumeParameterName, LinearToDecibles(volume));
    }

    public void SetSFXMute(bool isMute) {sfxSource.mute = isMute;}
    #endregion

    // ====================================================================================================
    //                     Helper Methods
    // ====================================================================================================
    #region Helper
    // Convert a linear value (0.0 - 1.0) to decibels
    private float LinearToDecibles(float linear)
    {
        if (linear <= 0f) return -80f;
        return 20f * Mathf.Log10(linear);
    }
    #endregion
}

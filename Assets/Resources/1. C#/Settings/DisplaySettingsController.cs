using UnityEngine;
using NovaSamples.UIControls;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

public class DisplaySettings : MonoBehaviour
{
    [Header("Component and Object")]
    [SerializeField] private Dropdown resolutionDropdown; // Not having resolution, too complex :(
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Settings Data")]
    [SerializeField] private List<Vector2Int> resolutionOptionSizeList = new List<Vector2Int>
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1920, 1200),
    };
    [SerializeField] private List<string> resolutionOptionTextList = new List<string>
    {
        "1920x1080",
        "1920x1200",
    };

    [Header("Initial Settings")]
    [SerializeField] private int resolutionIndex = 0;
    [SerializeField] private bool isFullscreen = true;

    // ====================================================================================================
    //                     Virtual Methods
    // ====================================================================================================
    #region Virtual
    private void Start()
    {
        // Assertion check
        if (resolutionDropdown)
        {
            Assert.IsTrue(resolutionOptionSizeList.Count > 0, "resolutionOptionSizeList is empty");
            Assert.IsTrue(resolutionOptionTextList.Count > 0, "resolutionOptionTextList is empty");
            Assert.IsTrue(
                resolutionOptionSizeList.Count == resolutionOptionTextList.Count,
                "resolutionOptionSizeList and resolutionOptionTextList does not match"
            );
        }
        // Connect UI events
        if (resolutionDropdown) resolutionDropdown.OnValueChanged.AddListener(
            value =>
            {
                resolutionIndex = resolutionOptionTextList.IndexOf(value);
                SaveSettings();
            }
        );
        if (fullscreenToggle) fullscreenToggle.OnToggled.AddListener(
            value =>
            {
                isFullscreen = value;
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
        // Set fullscreen
        Screen.fullScreen = isFullscreen;
        // Set resoultion
        Vector2Int currentResolution = resolutionOptionSizeList[resolutionIndex];
        Screen.SetResolution(currentResolution.x, currentResolution.y, Screen.fullScreen);
    }

    public void SaveSettings()
    {
        // Save settings
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        // Apply new saved changes
        ApplySettings();
    }

    public void LoadSettings()
    {
        // Load settings
        resolutionIndex = Math.Clamp(
            PlayerPrefs.GetInt("Resolution", 0), 0, resolutionOptionSizeList.Count
        );
        isFullscreen = PlayerPrefs.GetInt("Fullscreen", 0) == 1;
        // Apply settings
        ApplySettings();
        // Set UI values
        fullscreenToggle.ToggledOn = isFullscreen;
    }
    #endregion
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Component and Object")]
    [SerializeField] private GameObject mainContent;
    [SerializeField] private GameObject settingsContent;
    [SerializeField] private GameObject creditsContent;
    [Header("Main Menu")]
    [SerializeField] private string playTargetSceneName = "Game Scene";
    [SerializeField] private AudioClip mainMenuMusicAudioClip;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    private void Start()
    {
        // Assertion check
        Debug.Assert(mainContent, "mainContent is missing");
        Debug.Assert(settingsContent, "settingsContent is missing");
        Debug.Assert(creditsContent, "creditsContent is missing");
        // Initialize
        mainContent.SetActive(true);
        settingsContent.SetActive(false);
        creditsContent.SetActive(false);
        AudioManager.Instance.PlayMusic(mainMenuMusicAudioClip);
    }
    #endregion

    // ====================================================================================================
    //                     Button Functions
    // ====================================================================================================
    #region Button
    public void OnPlay()
    {
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(playTargetSceneName);
    }

    public void OnSettings()
    {
        mainContent.SetActive(false);
        settingsContent.SetActive(true);
        creditsContent.SetActive(false);
    }

    public void OnCredits()
    {
        mainContent.SetActive(false);
        settingsContent.SetActive(false);
        creditsContent.SetActive(true);
    }

    public void OnQuit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnBack()
    {
        mainContent.SetActive(true);
        settingsContent.SetActive(false);
        creditsContent.SetActive(false);
    }
    #endregion
}

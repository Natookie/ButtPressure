using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [Header("Component and Object")]
    [SerializeField] private GameObject content;
    [Header("Scene")]
    [SerializeField] private string restartTargerSceneName = "GameScene";
    [SerializeField] private string backToMenuTargetSceneName = "MainMenuScene";

    private Keyboard keyboard;
    
    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    private void Start()
    {
        // Assertion check
        Debug.Assert(content, "content is missing");
        // Initialize
        keyboard = Keyboard.current;
        content.SetActive(false);
    }

    private void Update()
    {
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (isGamePaused()) ClosePauseScreen();
            else OpenPauseScreen();
        }
    }
    #endregion

    // ====================================================================================================
    //                     Pause Functions
    // ====================================================================================================
    #region Pause
    public void OpenPauseScreen()
    {
        Time.timeScale = 0.0f;
        content.SetActive(true);
    }

    public void ClosePauseScreen()
    {
        Time.timeScale = 1.0f;
        content.SetActive(false);
    }

    public void OnRestart()
    {
        Time.timeScale = 1.0f;
        // AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(restartTargerSceneName);
    }

    public void OnBackToMenu()
    {
        Time.timeScale = 1.0f;
        // AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(backToMenuTargetSceneName);
    }

    public bool isGamePaused() {return Time.timeScale == 0.0f;}
    #endregion
}

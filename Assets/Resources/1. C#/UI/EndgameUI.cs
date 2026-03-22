using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameUI : MonoBehaviour
{
    [Header("Component and Object")]
    [SerializeField] private GameObject gameoverUIContent;
    [SerializeField] private GameObject gamewonUIContent;
    [Header("Scene")]
    [SerializeField] private string restartTargerSceneName = "GameScene";
    [SerializeField] private string backToMenuTargetSceneName = "MainMenuScene";
    [Header("Audio")]
    [SerializeField] private AudioClip pooSfxClip;
    
    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    private void Start()
    {
        // Assertion check
        Debug.Assert(gameoverUIContent, "gameoverUIContent is missing");
        Debug.Assert(gamewonUIContent, "gamewonUIContent is missing");
        // Initialize
        gameoverUIContent.SetActive(false);
        gamewonUIContent.SetActive(false);
    }
    #endregion

    // ====================================================================================================
    //                     Gameover Functions
    // ====================================================================================================
    #region Gameover
    public void OpenGameoverScreen()
    {
        // Check if already opened
        if (gameoverUIContent.activeSelf) return;
        // Open
        gameoverUIContent.SetActive(true);
        AudioManager.Instance.PlaySFX(pooSfxClip);
    }

    public void CloseGameoverScreen()
    {
        gameoverUIContent.SetActive(false);
    }

    public void OpenGamewonScreen()
    {
        // Check if already opened
        if (gamewonUIContent.activeSelf) return;
        // Open
        gamewonUIContent.SetActive(true);
        AudioManager.Instance.PlaySFX(pooSfxClip);
    }

    public void CloseGamewonScreen()
    {
        gamewonUIContent.SetActive(false);
    }

    public void OnRestart()
    {
        Time.timeScale = 1.0f;
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(restartTargerSceneName);
    }

    public void OnBackToMenu()
    {
        Time.timeScale = 1.0f;
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(backToMenuTargetSceneName);
    }
    #endregion
}

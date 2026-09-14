using UnityEngine;
using System.Collections;

public class RichKid : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private InteractableComponent interactableComponent;
    public Transform cameraFollow;
    [SerializeField] private MathMinigameController mathMinigameContorller;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    private void Start()
    {
        // Assertion check
        Debug.Assert(interactableComponent, "interactableComponent is missing");
        Debug.Assert(cameraFollow, "cameraFollow is missing");
        Debug.Assert(mathMinigameContorller, "mathMinigameContorller is missing");
        // Connect signal 
        EventFlag.Instance.MathMinigameFinished.AddListener(OnMinigameFinished);
    }
    #endregion

    // ====================================================================================================
    //                     Interact Functions
    // ====================================================================================================
    #region Interact 
    public void Interact() {StartCoroutine(DoHomework());}

    public void SetInteractableActive(bool value) => interactableComponent.CanInteract = value;

    private void OnMinigameFinished()
    {
        EventFlag.Instance.hasMoney = true;
        SetInteractableActive(false);
    }
    #endregion

    // ====================================================================================================
    //                     Dialogue Functions
    // ====================================================================================================
    #region Dialogue 
    private IEnumerator DoHomework(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Sure, let me do it."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();

        mathMinigameContorller.gameObject.SetActive(true);
        mathMinigameContorller.StartMinigame();
    }
    #endregion
}
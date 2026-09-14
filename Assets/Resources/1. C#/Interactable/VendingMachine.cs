using UnityEngine;
using System.Collections;

public class VendingMachine : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private InteractableComponent interactableComponent;
    [SerializeField] private RichKid richKid;

    [Header("AUDIO")]
    [SerializeField] private AudioClip vendingMachineSfx;
    [SerializeField] private float vendingMachineSfxTime = 6.8f;
 
    [HideInInspector] public bool canInteract;
    private bool hasMoney => EventFlag.Instance.hasMoney;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    private void Start()
    {
        // Assertion check
        Debug.Assert(interactableComponent, "interactableComponent is missing");
        Debug.Assert(richKid, "richKid is missing");
        // Connect event
        EventFlag.Instance.BullyQuestStarted.AddListener(()=>{SetInteractableActive(true);});
        EventFlag.Instance.MathMinigameFinished.AddListener(OnMinigameFinished);
    }
    #endregion

    // ====================================================================================================
    //                     Interact Functions
    // ====================================================================================================
    #region Interact
    public void Interact(){
        if(!hasMoney) StartCoroutine(Zero());
        else StartCoroutine(First());
    }

    public void SetInteractableActive(bool value) => interactableComponent.CanInteract = value;

    private void OnMinigameFinished()
    {
        SetInteractableActive(true);
    }
    #endregion

    // ====================================================================================================
    //                     Dialogue Functions
    // ====================================================================================================
    #region Dialogue
    IEnumerator Zero(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I don't have any money on me. What do i do?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(richKid.cameraFollow);
        DialogueManager.Instance.SetDialogue(
            DLib.YAMATO,
            "HEY! NERD GUY! The one in front of the vending\nmachine."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(richKid.cameraFollow);
        DialogueManager.Instance.SetDialogue(
            DLib.YAMATO,
            "You look hella broke, and you seems like ultra nerd."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.YAMATO,
            "I really need your help right now, i forgot to do\nmy math homework."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.YAMATO,
            "I will give you <color=#E76F2E>120 Yen</color> if you at least answer\n<color=#2FA4D7>6 questions correctly</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
        richKid.SetInteractableActive(true);
        SetInteractableActive(false);
        CameraController.Instance.ResetFollowTarget();
    }

    IEnumerator First(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "..."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        AudioManager.Instance.PlaySFX(vendingMachineSfx);
        yield return new WaitForSeconds(vendingMachineSfxTime);

        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "This onyo bought a red cola."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
        SetInteractableActive(false);
        ObjectiveUI.Instance.SetObjective("Deliver the red cola");
        EventFlag.Instance.hasDrink = true;
    }
    #endregion
}
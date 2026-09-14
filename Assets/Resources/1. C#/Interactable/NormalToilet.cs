using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InteractableComponent))]
public class NormalToilet : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private InteractableComponent interactableComponent;

    [Header("TOILET SETTINGS")]
    public bool hasBeenComplied;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Start()
    {
        // Assertion check
        Debug.Assert(interactableComponent, "interactableComponent is missing");
        // Connect events
        EventFlag.Instance.JanitorMoved.AddListener(OnJanitorMoved);
    }
    #endregion

    // ====================================================================================================
    //                     Interact Functions
    // ====================================================================================================
    #region Interact
    public void Interact(){
        if(hasBeenComplied) StartCoroutine(AskJanitorWhereabout());
        else StartCoroutine(TriggerComplaint());
    }

    public void SetInteractableActive(bool value) => interactableComponent.CanInteract = value;
    #endregion

    // ====================================================================================================
    //                     Dialogue Functions
    // ====================================================================================================
    #region Dialogue
    IEnumerator TriggerComplaint(){
        yield return new WaitForSeconds(0.1f);
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "*Click *Click ~~*Click*Click*Click\nIt's locked, the <color=#E76F2E>janitor</color> should has the key."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        ObjectiveUI.Instance.SetObjective("Ask for a key to the janitor");

        DialogueManager.Instance.HideDialogueUI();
        EventFlag.Instance.isJanitorQuestStarted = true;
        SetInteractableActive(false);
    }

    IEnumerator AskJanitorWhereabout(){
        yield return new WaitForSeconds(0.1f);
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "*Click *Click. It's still locked.\nWhere even is the janitor anyway?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "He just dissapeared into thin air. HMMM."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Aishh Shibal.~ Fuzakena janitor chan.\nI need to find another toilet"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        ObjectiveUI.Instance.SetObjective("Go to the 1st floor's toilet");

        DialogueManager.Instance.HideDialogueUI();
        SetInteractableActive(false);
        Debug.Log("done");
        // stairway.enabled = true;
    }
    #endregion

    // ====================================================================================================
    //                     Event Functions
    // ====================================================================================================
    #region Event
    private void OnJanitorMoved()
    {
        SetInteractableActive(true);
        hasBeenComplied = true;
    }
    #endregion
}
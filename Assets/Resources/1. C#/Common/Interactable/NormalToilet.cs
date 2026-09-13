using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InteractableObject))]
public class NormalToilet : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    public bool hasBeenComplied;
    [SerializeField] private JanitorQuest janitor;
    [SerializeField] private InteractableObject stairway;

    private InteractableObject io;

    void Start(){
        io = GetComponent<InteractableObject>();
    }

    public void Interact(){
        if(hasBeenComplied) StartCoroutine(AskJanitorWhereabout());
        else StartCoroutine(TriggerComplaint());
    }

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
        janitor.SetAskForKey();
        janitor.GetComponent<InteractableObject>().enabled = true;
        io.SelfDestruct();
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
        io.SelfDestruct();
        stairway.enabled = true;
    }
}
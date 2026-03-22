using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InteractableObject))]
public class VendingMachine : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private BullyQuest quest;
    [SerializeField] private RichKid richKid;
    [SerializeField] private InteractableObject interactable;
 
    private bool hasMoney;

    [HideInInspector] public bool canInteract;

    public void SetInteractableActive(bool value) => interactable.enabled = value;
    public void SetHasMoney() => hasMoney = true;

    public void Interact(){
        if(!hasMoney) StartCoroutine(Zero());
        else StartCoroutine(First());
    }
    
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

        PlayerCam.Instance.FaceTarget(richKid.gameObject.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.YAMATO,
            "HEY! NERD GUY! The one in front of the vending\nmachine."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.FocusOnTarget(richKid.gameObject.transform);
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
        richKid.GetComponent<InteractableObject>().enabled = true;
        GetComponent<InteractableObject>().SelfDestruct();
        PlayerCam.Instance.ReturnToPlayer();

        InteractableObject io = GetComponent<InteractableObject>();
        io.SetPrompt("Buy a drink");
    }

    IEnumerator First(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "This onyo bought a red cola."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
        GetComponent<InteractableObject>().SelfDestruct();
        quest.SetHasDrink(true);
    }
}
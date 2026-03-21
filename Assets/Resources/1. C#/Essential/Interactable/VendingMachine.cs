using UnityEngine;
using System.Collections;

public class VendingMachine : MonoBehaviour, IMultiInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private Homework homework;
    [SerializeField] private BullyQuest quest;
    [SerializeField] private InteractableObject interactable;
 
    private bool hasMoney;
    private bool firstExecuted;
    private bool minigameCompleted;

    [HideInInspector] public bool canInteract;

    public void SetInteractableActive(bool value) => interactable.enabled = value;

    public void FirstInteraction(){
        if(firstExecuted) return;

        InteractableObject io = GetComponent<InteractableObject>();
        StartCoroutine(Zero());
        io.SetPrompt("Buy a drink");
    }
    
    public void SubsequentInteraction(){
        if(minigameCompleted) return;
        if(!firstExecuted){
            StartCoroutine(First());
            return;
        }
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

        PlayerCam.Instance.FocusOnTarget(homework.gameObject.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Hey, there is a stupid looking guy over there."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Maybe i can help him do some of his homework.\n~Then i will ask for money. ~yay!"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.HideDialogueUI();
        firstExecuted = true;
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
        quest.SetHasDrink(true);
        minigameCompleted = true;
    }
}
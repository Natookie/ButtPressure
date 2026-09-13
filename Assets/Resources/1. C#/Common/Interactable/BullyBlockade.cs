using UnityEngine;
using System.Collections;

public class BullyBlockade : MonoBehaviour, IInteractable
{
    [Header("QUEST TWEAK")]
    [SerializeField] private BoxCollider2D barrier;
    [SerializeField] private BoxCollider2D triggerDialogue;
    [SerializeField] private InteractableObject interactableComponent;
    [SerializeField] private GameObject stairway;
    
    [HideInInspector] public bool hasIntroduced;
    private bool hasTotem => true;
    // PlayerInteraction.Instance.hasTotem;

    void Start(){
        interactableComponent = GetComponent<InteractableObject>();
        interactableComponent.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D coll){
        if(hasTotem){
            if(!hasIntroduced){
                if(!DialogueManager.Instance.IsTyping) StartCoroutine(CompleteQuestTwo());
                stairway.SetActive(true);
                hasIntroduced = true;
            }else VisualCue.Instance.SetCurrentInteractable(interactableComponent);
            return;
        }

        if(coll.CompareTag("Player") && !hasIntroduced){
            if(!DialogueManager.Instance.IsTyping) StartCoroutine(IntroduceProblem());
            return;
        }
    }

    public void Interact() => StartCoroutine(CompleteQuest());

    IEnumerator IntroduceProblem(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.SATO,
            "Whoa whoa whoa! Where do you think you're going?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Just looking around?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ChangeFollowTarget(this.transform);
        CameraShake.Instance.ShakeCamera(false);
        DialogueManager.Instance.SetDialogue(
            DLib.SATO,
            "CAFETARIA YOU SAID? HAH! Look at this nerd trying\nto get some food."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.SATO,
            "You think you can just walk past us? This is\nOUR territory."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I never said cafetaria."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.SATO,
            "Listen here, nerd. The cafeteria is for cool kids only."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "(Ugh.. i better be going, idk what his problem is)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        hasIntroduced = true;
    }

    IEnumerator CompleteQuest(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.SATO,
            "Didn't i said that CAFETARIA is for cool kids only?\nWhy are you still he-|"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Nuh uh, look what i have in my hand? A totem.\nA coooool totem. Does that mean i can pas now?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.SetDialogue(
            DLib.SATO,
            "Ye. That's John Kaisen's totem."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "What an absolute proper conversation."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "(Finally, i can take a <color=#8C5A3C>poopie</color>)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.HideDialogueUI();
        if(barrier != null) barrier.enabled = false;
        ObjectiveUI.Instance.SetObjective("Go to the 2nd floor's toilet");
        stairway.SetActive(true);

        if(interactableComponent != null) interactableComponent.SelfDestruct();
    }

    IEnumerator CompleteQuestTwo(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.SATO,
            "Sick totem! Looks just like John Kaisen's"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "(This Sato guy seems chill)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.HideDialogueUI();
        if(barrier != null) barrier.enabled = false;
        ObjectiveUI.Instance.SetObjective("Go to the 2nd floor's toilet");
        
        if(interactableComponent != null) interactableComponent.SelfDestruct();
    }
}
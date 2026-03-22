using UnityEngine;
using System.Collections;

public class CrowdBlockade : MonoBehaviour, IInteractable
{
    [Header("QUEST TWEAK")]
    [SerializeField] private BoxCollider2D barrier;
    [SerializeField] private InteractableObject interactableComponent;
    [SerializeField] private InteractableObject cafeteriaDoorInteractable;

    [HideInInspector] public bool hasIntroduced;

    void Start(){
        interactableComponent = GetComponent<InteractableObject>();
        if(interactableComponent == null) interactableComponent = gameObject.AddComponent<InteractableObject>();
            
        interactableComponent.enabled = false;
        cafeteriaDoorInteractable.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D coll){
        if(coll.CompareTag("Player") && !DialogueManager.Instance.IsTyping){
            if(!hasIntroduced) StartCoroutine(IntroduceProblem());
            else StartCoroutine(RemindProblem());
        }
    }

    public void Interact() => StartCoroutine(RemindProblem());

    IEnumerator IntroduceProblem(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.CROWD,
            "(Crowd sounds crowding)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Excuse me, can I please get through?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.CROWD,
            "(Crowd sounds crowding)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I can't get through, maybe I'll go around."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.CROWD,
            "(Crowd sounds crowding)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.HideDialogueUI();
        ObjectiveUI.Instance.SetObjective("Find a way to pass the crowd");
        hasIntroduced = true;
        cafeteriaDoorInteractable.enabled = true;
    }

    IEnumerator RemindProblem(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.CROWD,
            "(Crowd sounds crowding)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.HideDialogueUI();
    }
}
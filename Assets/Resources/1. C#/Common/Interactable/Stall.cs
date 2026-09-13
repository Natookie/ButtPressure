using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InteractableObject))]
public class Stall : MonoBehaviour, IInteractable
{
    [Header("TWEAKS")]
    [SerializeField] private bool hasHanako;

    [SerializeField] private Transform hanakoSpawnPoint;
    private EvtManager evtManager;

    void Start(){
        if(hasHanako){
            evtManager = FindObjectOfType<EvtManager>();
            if(evtManager == null) Debug.LogWarning("Stall: EvtManager not found in scene!");
        }
    }

    public void Interact(){
        if(hasHanako) StartCoroutine(SpecialDialogue());
        else StartCoroutine(NormalDialogue());
    }

    IEnumerator NormalDialogue(){
        yield return new WaitForSeconds(0.1f);
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Locked"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
    }

    IEnumerator SpecialDialogue(){
        yield return new WaitForSeconds(0.1f);
        
        Hanako hanako = Hanako.Instance;
        if(hanako != null && hanakoSpawnPoint != null){
            hanako.transform.position = hanakoSpawnPoint.position;
        }
        
        CameraController.Instance.ChangeFollowTarget(hanakoSpawnPoint);
        yield return new WaitForSeconds(1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);

        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO,
            "<color=#DA4848>RAWRRRRRR..</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "HOLY.. S**T!!~ Is this truly my kisah?~"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(hanako.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO,
            "<color=#DA4848>Omae no shita o hikisaite yaru.</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Mizu and kimchi onegai si mas. I don't speak nihongo\n Please don't touch me."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        
        if(hanako != null) hanako.canMove = true;
        evtManager.teleportHanako = true;
        evtManager.allowTrigger = true;
        ObjectiveUI.Instance.SetObjective("Survive, go to the second floor");
    }
}
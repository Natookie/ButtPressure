using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InteractableObject))]
public class Stall : MonoBehaviour, IInteractable
{
    [Header("TWEAKS")]
    [SerializeField] private bool hasHanako;

    [SerializeField] private GameObject hanakoObj;
    [SerializeField] private Transform hanakoSpawnPoint;
    [SerializeField] private EvtManager evtManager;

    private GameObject hanako;

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
        SpawnHanako();
        PlayerCam.Instance.FocusOnTarget(hanakoSpawnPoint, Vector3.zero);
        yield return new WaitForSeconds(1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);

        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO,
            "<color=#DA4848>RAWRRRRRR..</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "HOLY.. S**T!!~ Is this truly my kisah?~"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.FocusOnTarget(hanako.transform, Vector3.zero);
        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO,
            "<color=#DA4848>Omae no shita o hikisaite yaru.</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Mizu and kimchi onegai si mas. I don't speak nihongo\n Please don't touch me."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        
        Hanako hanakoScript = hanako.GetComponent<Hanako>();
        hanakoScript.canMove = true;
        
        ObjectiveUI.Instance.SetObjective("Survive, go to the second floor");
    }

    void SpawnHanako(){
        if(hanakoObj == null) return;
        
        Transform grandparent = transform.parent?.parent;
        if(grandparent == null) return;
        
        hanako = Instantiate(hanakoObj, hanakoSpawnPoint.position, Quaternion.identity);
        hanako.transform.SetParent(grandparent);
    }
}
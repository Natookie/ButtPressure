using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InteractableObject))]
public class Stall : MonoBehaviour, IInteractable
{
    [Header("TWEAKS")]
    [SerializeField] private bool hasHanako;

    public void Interact(){
        if(hasHanako) StartCoroutine(SpecialDialogue());
        else StartCoroutine(NormalDialogue());
    }

    IEnumerator NormalDialogue(){
        yield return new WaitForSeconds(0.1f);
        DialogueManager.Instance.ShowDialogueUI();

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Locked"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
    }

    IEnumerator SpecialDialogue(){
        yield return new WaitForSeconds(0.1f);
        DialogueManager.Instance.ShowDialogueUI();

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Test"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
    }
}
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InteractableObject))]
public class RichKid : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private MathMinigameController mmg;

    public void Interact(){
        StartCoroutine(SomeBullShit());
    }

    IEnumerator SomeBullShit(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Sure, let me do it."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();

        mmg.gameObject.SetActive(true);
        mmg.StartMinigame();
    }
}
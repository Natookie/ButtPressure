using UnityEngine;
using System.Collections;

public class JanitorQuest : MonoBehaviour
{
    [Header("QUEST TWEAK")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Door targetStairDoor;
    [SerializeField] private bool hasKey;

    [HideInInspector] private bool hasIntroduced;

    private void Start()
    {
        spriteRenderer.enabled = false;
    }

    public void SetHasKey(bool value) => hasKey = value;

    public void EnterRoom(){
        if(!hasKey){
            if(!hasIntroduced) StartCoroutine(TriggerQuest());
            else StartCoroutine(RemindQuest());
        }
        else StartCoroutine(CompleteQuest());
    }

    public IEnumerator TriggerQuest(){
        yield return new WaitForSeconds(0.1f);

        spriteRenderer.enabled = true;

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);

        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "Apdpsdocnwdidja..."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "What?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "Kyaaeedndidnocngettooo, piahsidohbasidh"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Speak up old man, I can't understa"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        PlayerCam.Instance.FocusOnTarget(this.transform);
        CameraShake.Instance.ShakeCamera(true);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "FIND ME THE DAMN KEYS YOU LITTLE BRAT!!!"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "....."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        hasIntroduced = true;
    }

    public IEnumerator CompleteQuest(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Here"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "..."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        ObjectiveUI.Instance.SetObjective("Go to the 2nd floor");

        DialogueManager.Instance.HideDialogueUI();

        spriteRenderer.enabled = false;
        targetStairDoor.isLocked = false;
    }

    public IEnumerator RemindQuest(){
        yield return new WaitForSeconds(0.1f);
        
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "....."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "...Keys??"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Not yet.."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "Then you better find it, otherwise I'll smack you to\npieces using my broom."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
    }
}
using UnityEngine;
using System.Collections;

public class JanitorQuest : MonoBehaviour, IInteractable
{
    [Header("QUEST TWEAK")]
    [SerializeField] private bool askForKey;
    [SerializeField] private bool hasComply;
    [SerializeField] private BoxCollider2D barrier;
    [SerializeField] private BoxCollider2D triggerDialogue;
    [Space(10)]
    [SerializeField] private InteractableObject interactableComponent;
    [SerializeField] private NormalToilet toilet;
    [SerializeField] private GameObject stairway;
    
    [Header("QUEST TWEAK")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform targetCafetaria;

    [HideInInspector] private bool hasIntroduced;

    public void SetAskForKey() => askForKey = true;

    void OnTriggerEnter2D(Collider2D coll){
        if(coll.CompareTag("Player") && !hasIntroduced){
            VisualCue.Instance.SetCurrentInteractable(interactableComponent);
        }
    }
    public void Interact(){
        if(!askForKey){
            if(hasIntroduced) return;
            StartCoroutine(Murmur());
        }
        else StartCoroutine(Comply());
    }

    public IEnumerator Murmur(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);

        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "Sushi salmon honda takoyaki..."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "This old geezer is a freak."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "Kyaaeedndidnocngettooo.\n~piahsidohbasidh, duh game jam nya ga selesai."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Whatever, i need to go to the restroom QUICK."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        interactableComponent.SelfDestruct();
        hasIntroduced = true;
    }

    public IEnumerator Comply(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Hey old geezer! Why is the restoom locked? I need \nto take a <color=#8C5A3C>poopie</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "Uhhh. I think i forgot to unlock it."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Give me the key then. It's very urgent."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        PlayerCam.Instance.FocusOnTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "I will open it myself, follow me to the toilet.\n*Wink wink"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
        "WHAT IN THE F-|"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        PlayerCam.Instance.ReturnToPlayer();
        ObjectiveUI.Instance.SetObjective("Follow the janitor to the restroom");

        DialogueManager.Instance.HideDialogueUI();
        toilet.GetComponent<InteractableObject>().enabled = true;
        toilet.hasBeenComplied = true;

        //Janitor Gerak() ke arah Right door cafetaria
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

    public void SetBarrier(bool value) => barrier.enabled = value;
}
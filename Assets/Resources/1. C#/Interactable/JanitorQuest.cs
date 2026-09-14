using UnityEngine;
using System.Collections;
using System;

public class JanitorQuest : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private InteractableComponent interactableComponent;
    public Transform cameraFollow;
    [SerializeField] private DoorPush cafeteriaDoorPush;

    [Header("QUEST TWEAK")]
    [SerializeField] private bool askForKey;
    [SerializeField] private bool hasComply;

    [Header("MOVE SETTINGS")]
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("ANIMATION")]
    [SerializeField] private string idleAnimationName = "JanitorIdle";
    [SerializeField] private string walkAnimationName = "JanitorWalk";

    [HideInInspector] private bool hasIntroduced;
    // Move
    private bool isMoving = false;
    private Vector3 moveTargetPosition;
    private bool isDisappearAfterMoving = false;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Start()
    {
        // Assertion check
        Debug.Assert(spriteRenderer, "spriteRenderer is missing");
        Debug.Assert(animator, "animator is missing");
        Debug.Assert(interactableComponent, "interactableComponent is missing");
        Debug.Assert(cameraFollow, "cameraFollow is missing");
        Debug.Assert(cafeteriaDoorPush, "cafeteriaDoorPush is missing");
        // Connect events
        EventFlag.Instance.JanitorQuestStarted.AddListener(SetAskForKey);
    }
    
    private void FixedUpdate(){
        if(isMoving){
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.MoveTowards(
                transform.position.x, moveTargetPosition.x, Time.fixedDeltaTime * moveSpeed
            );
            transform.position = newPosition;
            spriteRenderer.flipX = Math.Sign(moveTargetPosition.x - transform.position.x) == -1;
            
            bool isComplete = true;
            if(Mathf.Abs(moveTargetPosition.x - transform.position.x) > 0.01f) isComplete = false;
            
            if(isComplete)
            {
                cafeteriaDoorPush.ToggleLock(false);
                isMoving = false;
                animator.Play(idleAnimationName);
                if(isDisappearAfterMoving) gameObject.SetActive(false);
            }
            else cafeteriaDoorPush.ToggleLock(true);

            SetInteractableActive(false);
        }
    }
    #endregion

    // ====================================================================================================
    //                     Interact Functions
    // ====================================================================================================
    #region Interact
    public void Interact(){
        if(!askForKey){
            if(hasIntroduced) return;
            StartCoroutine(Murmur());
        }
        else StartCoroutine(Comply());
    }

    public void SetInteractableActive(bool value) => interactableComponent.CanInteract = value;

    public void MoveToTarget(Vector3 targetPosition, bool disappearAfterMoving = false)
    {
        moveTargetPosition = targetPosition;
        isMoving = true;
        isDisappearAfterMoving = disappearAfterMoving;
        animator.Play(walkAnimationName);
    }

    public void SetAskForKey()
    {
        askForKey = true;
        SetInteractableActive(true);
    }
    #endregion

    // ====================================================================================================
    //                     Dialogue Functions
    // ====================================================================================================
    #region Dialogue
    public IEnumerator Murmur(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);

        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "Sushi salmon honda takoyaki..."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "This old geezer is a freak."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "Kyaaeedndidnocngettooo.\n~piahsidohbasidh, duh game jam nya ga selesai."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Whatever, i need to go to the restroom QUICK."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        SetInteractableActive(false);
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
        
        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
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
        
        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
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

        CameraController.Instance.ResetFollowTarget();
        ObjectiveUI.Instance.SetObjective("Follow the janitor to the restroom");

        DialogueManager.Instance.HideDialogueUI();

        EventFlag.Instance.hasJanitorMoved = true;

        //Janitor Gerak() ke arah Right door cafetaria
        MoveToTarget(cafeteriaDoorPush.transform.position, true);
    }

    public IEnumerator RemindQuest(){
        yield return new WaitForSeconds(0.1f);
        
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "....."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "...Keys??"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Not yet.."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JANITOR,
            "Then you better find it, otherwise I'll smack you to\npieces using my broom."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
    }
    #endregion
}
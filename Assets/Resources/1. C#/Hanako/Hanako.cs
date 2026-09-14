using UnityEngine;
using System.Collections;

public class Hanako : MonoBehaviour
{
    public static Hanako Instance {get; private set;}

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator animator;
    public Transform cameraFollow;
    [SerializeField] private Transform chaseEndPosition;
    [SerializeField] private TriggerComponent hanakoTrigger;

    [Header("MOVEMENT SETTINGS")]
    [SerializeField] private float moveSpeed = 6.0f;
    [SerializeField] private float stoppingDistance = 0.2f;

    [Header("ANIMATION")]
    [SerializeField] private string walkingParameterName = "isWalking";

    [Header("AUDIO")]
    [SerializeField] private AudioClip ScreamSfxClip;

    public float horizontalDirection;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy() => Instance = null;

    void Start()
    {
        // Assertion check
        Debug.Assert(rb, "rb is missing");
        Debug.Assert(col, "col is missing");
        Debug.Assert(sr, "sr is missing");
        Debug.Assert(animator, "animator is missing");
        Debug.Assert(cameraFollow, "cameraFollow is missing");
        Debug.Assert(chaseEndPosition, "chaseEndPosition is missing");
        Debug.Assert(hanakoTrigger, "hanakoTrigger is missing");
        Debug.Assert(ScreamSfxClip, "ScreamSfxClip is empty");
        // Connect events
        hanakoTrigger.PlayerEnter.AddListener(()=>StartCoroutine(SpawnHanako()));
        // Initialize
        HideHanako();
    }

    void Update()
    {
        // Check is already reached the end
        float offset = Mathf.Abs(chaseEndPosition.transform.position.x - transform.position.x);
        if(offset <= stoppingDistance) EndChase();
        // Update sprite
        if(horizontalDirection > 0) sr.flipX = true;
        else if(horizontalDirection < 0) sr.flipX = false;
        // Update animation
        animator.SetBool(walkingParameterName, rb.linearVelocity != Vector2.zero);
    }

    void FixedUpdate()
    {
        Vector2 newVelocity = new Vector2(horizontalDirection * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity = newVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EndChase();
            StartCoroutine(CaughtSequenceBareHand());
        }
    }
    #endregion

    // ====================================================================================================
    //                     Hanako Functions
    // ====================================================================================================
    #region Hanako
    public void ShowHanako()
    {
        sr.enabled = true;
    }

    public void HideHanako()
    {
        sr.enabled = false;
    }

    public void StartChase()
    {
        horizontalDirection = 1;
    }

    public void EndChase()
    {
        horizontalDirection = 0;
        rb.linearVelocity = Vector2.zero;
    }
    #endregion

    // ====================================================================================================
    //                     Dialogue Functions
    // ====================================================================================================
    #region Dialogue
    IEnumerator SpawnHanako()
    {
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);

        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(DLib.PLAYER, ".");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        ShowHanako();
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, ".");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(DLib.HANAKO, ".....");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        AudioManager.Instance.PlaySFX(ScreamSfxClip);
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "AAAAAAAAAAAAAAAGGGGHHHHHHHHHH");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        CameraController.Instance.ResetFollowTarget();
        StartChase();
        hanakoTrigger.gameObject.SetActive(false);
    }

    IEnumerator CaughtSequenceBareHand(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "Excuse me mate.");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "I need you to help me clean the toilet.");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(DLib.PLAYER, "NOOOOO!");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "Bruv. you're not busy innit mate?");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraShake.Instance.ShakeCamera(true);
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "<color=#DA4848>LET'S GOOOO!!!!!</color>");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        GameManager.Instance.EndGame(2);
    }

    // UNUSED!
    IEnumerator CaughtSequenceInLocker(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "Udin, Anterin gw ke Puri dong");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "Ekhem, maksud nya-|");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraShake.Instance.ShakeCamera(true);
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "<color=#DA4848>ISSHO NI AKUINATO O MIYOU!!!!!</color>");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        GameManager.Instance.EndGame(2);
    }

    // UNUSED!
    IEnumerator TauntReaction(){
        // float directionToPlayer = transform.position.x > playerTransform.position.x ? 1 : -1;
        
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);

        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO, 
            "Oh my... it's been a long time."
        );
        
        // while(DialogueManager.Instance.IsTypingActive()){
        //     directionToPlayer = transform.position.x > playerTransform.position.x ? 1 : -1;
        //     if(playerSr != null) playerSr.flipX = directionToPlayer < 0;
        //     yield return null;
        // }
        
        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO, 
            "I miss this sound."
        );
        
        // while(DialogueManager.Instance.IsTypingActive()){
        //     directionToPlayer = transform.position.x > playerTransform.position.x ? 1 : -1;
        //     if(playerSr != null) playerSr.flipX = directionToPlayer < 0;
        //     yield return null;
        // }

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER, 
            "Huh? she is not attacking me?\nI accidentaly knocked the clock. <color=#E76F2E>I will keep that in mind.</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        // hasIntroduced = true;
        DialogueManager.Instance.HideDialogueUI();
    }
    #endregion
}
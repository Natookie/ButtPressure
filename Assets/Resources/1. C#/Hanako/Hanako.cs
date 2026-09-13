using UnityEngine;
using System.Collections;

public class Hanako : MonoBehaviour
{
    public static Hanako Instance {get; private set;}

    [Header("TWEAKS")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float slowDownDistance = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float catchDistance = 0.5f;

    [Header("PATROL")]
    [SerializeField] private float idleTime = 2f;
    [SerializeField] private float tauntIdleTime = 10f;

    [Header("ANIMATION")]
    [SerializeField] private Sprite[] animSprites;
    [SerializeField] private float frameRate = 10f;

    private SpriteRenderer playerSr;
    private SpriteRenderer ghostSr;

    public bool canMove = false;
    public bool hasTeleported;
    private bool hasIntroduced = false;

    private Transform playerTransform;
    private Vector3 lastKnownPlayerPosition;
    private bool playerDetected;
    private bool playerInLocker;
    private float patrolDirection = 1f;
    private float patrolTimer;
    private BoxCollider2D boundary;
    private bool hasReachedLocker;
    private bool isCaught = false;
    private bool isTaunting = false;
    private Vector3 tauntTargetPosition;
    private float tauntTimer;

    // Animation
    private Coroutine animationCoroutine;
    private bool isMoving = false;

    void Awake(){
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start(){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) playerTransform = player.transform;

        ghostSr = this.gameObject.GetComponent<SpriteRenderer>();
        playerSr = player.GetComponent<SpriteRenderer>();

        // Set initial sprite
        if(animSprites != null && animSprites.Length > 0 && ghostSr != null)
            ghostSr.sprite = animSprites[0];
    }

    void Update(){
        if(!canMove || playerTransform == null || isCaught) return;

        playerInLocker = !playerSr.enabled;

        if(isTaunting){
            HandleTauntState();
            return;
        }
        
        DetectPlayer();
        
        float currentSpeed = CalculateSpeed();
        MoveTowardsTarget(currentSpeed);
        DrawDebugRay(currentSpeed);

        UpdateAnimation(currentSpeed > 0);
    }

    void UpdateAnimation(bool moving){
        if(moving && !isMoving){
            isMoving = true;
            StartAnimation();
        }
        else if(!moving && isMoving){
            isMoving = false;
            StopAnimation();
        }
    }
    
    void StartAnimation(){
        if(animationCoroutine != null){
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        if(animSprites != null && animSprites.Length > 0){
            animationCoroutine = StartCoroutine(AnimateSprites());
        }
    }

    void StopAnimation(){
        if(animationCoroutine != null){
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        
        // Reset to first frame when stopped
        if(animSprites != null && animSprites.Length > 0 && ghostSr != null)
            ghostSr.sprite = animSprites[0];
    }

    IEnumerator AnimateSprites(){
        int currentFrame = 0;
        
        while(isMoving && animSprites != null && animSprites.Length > 0){
            if(ghostSr != null && animSprites[currentFrame] != null)
                ghostSr.sprite = animSprites[currentFrame];
            
            currentFrame = (currentFrame + 1) % animSprites.Length;
            yield return new WaitForSeconds(1f / frameRate);
        }
        
        animationCoroutine = null;
    }
    
    void HandleTauntState(){
        float distanceToTarget = Mathf.Abs(transform.position.x - tauntTargetPosition.x);

        if(distanceToTarget > 0.1f){
            float direction = tauntTargetPosition.x > transform.position.x ? 1 : -1;
            if(ghostSr != null) ghostSr.flipX = direction > 0;
            if(playerSr != null) playerSr.flipX = direction < 0;
            
            float newX = transform.position.x + (direction * walkSpeed * Time.deltaTime);
            
            if(direction > 0 && newX > tauntTargetPosition.x) newX = tauntTargetPosition.x;
            else if(direction < 0 && newX < tauntTargetPosition.x) newX = tauntTargetPosition.x;
            
            if(boundary != null) newX = Mathf.Clamp(newX, boundary.bounds.min.x + 0.5f, boundary.bounds.max.x - 0.5f);
            
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        else{
            if(!hasIntroduced) return;

            if(tauntTimer <= 0f) tauntTimer = tauntIdleTime;
            else{
                tauntTimer -= Time.deltaTime;
                if(tauntTimer <= 0f) isTaunting = false;
            }
        }
    }

    void DetectPlayer(){
        if(playerInLocker){
            playerDetected = false;
            return;
        }

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distance = directionToPlayer.magnitude;
        
        if(distance > detectionRange){
            playerDetected = false;
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, distance, playerLayer);
        
        if(hit.collider != null && hit.collider.CompareTag("Player")){
            playerDetected = true;
            lastKnownPlayerPosition = playerTransform.position;
            hasReachedLocker = false;
            
            if(DialogueManager.Instance != null && DialogueManager.Instance.IsTyping)
                DialogueManager.Instance.ForceStopDialogue();
        }
        else playerDetected = false;
    }

    float CalculateSpeed(){
        if(playerDetected) return runSpeed;
        else if(lastKnownPlayerPosition != Vector3.zero){
            float distanceToLastKnown = Mathf.Abs(transform.position.x - lastKnownPlayerPosition.x);
            
            if(distanceToLastKnown < slowDownDistance){
                if(distanceToLastKnown < 0.1f){
                    lastKnownPlayerPosition = Vector3.zero;
                    return 0f;
                }
                return walkSpeed;
            }
            return walkSpeed;
        }
        else return Patrol();
    }

    float Patrol(){
        if(lastKnownPlayerPosition != Vector3.zero || playerDetected) return 0f;
            
        patrolTimer += Time.deltaTime;
        
        if(patrolTimer >= idleTime){
            patrolDirection *= -1f;
            patrolTimer = 0f;
        }
        
        float newX = transform.position.x + (patrolDirection * walkSpeed * Time.deltaTime);
        
        if(boundary != null) newX = Mathf.Clamp(newX, boundary.bounds.min.x + 0.5f, boundary.bounds.max.x - 0.5f);
        
        if(Mathf.Abs(newX - transform.position.x) < 0.01f) patrolTimer = idleTime;
        
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        if(ghostSr != null) ghostSr.flipX = patrolDirection > 0;
        
        return walkSpeed;
    }

    void MoveTowardsTarget(float speed){
        if(speed == 0f) return;
        
        Vector3 targetPosition;
        
        if(playerDetected) targetPosition = playerTransform.position;
        else if(lastKnownPlayerPosition != Vector3.zero) targetPosition = lastKnownPlayerPosition;
        else return;

        float direction = targetPosition.x > transform.position.x ? 1 : -1;
        
        if(ghostSr != null) ghostSr.flipX = direction > 0;
        float newX = transform.position.x + (direction * speed * Time.deltaTime);
        
        if(direction > 0 && newX > targetPosition.x) newX = targetPosition.x;
        else if(direction < 0 && newX < targetPosition.x) newX = targetPosition.x;
        
        if(boundary != null) newX = Mathf.Clamp(newX, boundary.bounds.min.x + 0.5f, boundary.bounds.max.x - 0.5f);
        
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        
        CheckIfCaught(targetPosition);
    }
    
    void CheckIfCaught(Vector3 targetPosition){
        float distanceToTarget = Mathf.Abs(transform.position.x - targetPosition.x);
        
        if(distanceToTarget <= catchDistance){
            if(playerDetected){
                if(playerInLocker){
                    if(!hasReachedLocker){
                        // PlayerMovement.Instance.HideInLocker(targetPosition, false);
                        hasReachedLocker = true;
                        StartCoroutine(CaughtSequenceInLocker());
                    }
                }
                else{
                    if(!hasReachedLocker){
                        hasReachedLocker = true;
                        StartCoroutine(CaughtSequenceBareHand());
                    }
                }
            }
        }
    }

    void DrawDebugRay(float currentSpeed){
        if(playerTransform == null) return;
        
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distance = directionToPlayer.magnitude;
        float actualDistance = Mathf.Min(distance, detectionRange);
        
        Color rayColor = currentSpeed == runSpeed ? Color.red : Color.yellow;
        Debug.DrawRay(transform.position, directionToPlayer.normalized * actualDistance, rayColor);
    }

    public void Taunt(Transform tauntPosition){
        if(tauntPosition == null || isTaunting || isCaught) return;
        
        tauntTargetPosition = tauntPosition.position;
        isTaunting = true;
        tauntTimer = 0f;
    }
    
    IEnumerator TauntReactionSequence(){
        float directionToPlayer = transform.position.x > playerTransform.position.x ? 1 : -1;
        
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ChangeFollowTarget(this.transform);

        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO, 
            "Oh my... it's been a long time."
        );
        
        while(DialogueManager.Instance.IsTypingActive()){
            directionToPlayer = transform.position.x > playerTransform.position.x ? 1 : -1;
            if(playerSr != null) playerSr.flipX = directionToPlayer < 0;
            yield return null;
        }
        
        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO, 
            "I miss this sound."
        );
        
        while(DialogueManager.Instance.IsTypingActive()){
            directionToPlayer = transform.position.x > playerTransform.position.x ? 1 : -1;
            if(playerSr != null) playerSr.flipX = directionToPlayer < 0;
            yield return null;
        }

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER, 
            "Huh? she is not attacking me?\nI accidentaly knocked the clock. <color=#E76F2E>I will keep that in mind.</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        hasIntroduced = true;
        DialogueManager.Instance.HideDialogueUI();
    }

    public void TeleportTo(Vector3 position){
        transform.position = position;
        hasTeleported = true;
        canMove = true;

        if(ghostSr != null) ghostSr.flipX = position.x > transform.position.x;
        
        GrandFatherClock clock = FindAnyObjectByType<GrandFatherClock>();
        if(clock != null){
            StartCoroutine(TeleportTauntSequence(clock.transform));
            clock.GetComponent<InteractableObject>().enabled = true;
            clock.enabled = true;
        }
        StartCoroutine(TauntReactionSequence());
    }
    
    IEnumerator TeleportTauntSequence(Transform clockTransform){
        yield return null;
        Taunt(clockTransform);
    }

    bool isPlayerInLocker() => playerSr.enabled;

    IEnumerator CaughtSequenceInLocker(){
        isCaught = true;
        canMove = false;

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

    IEnumerator CaughtSequenceBareHand(){
        isCaught = true;
        canMove = false;

        yield return new WaitForSeconds(0.1f);
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "Excuse me mate.");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "I need you to help me clean the toilet.");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(DLib.PLAYER, "NOOOOO!");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "Bruv. you're not busy innit mate?");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraShake.Instance.ShakeCamera(true);
        DialogueManager.Instance.SetDialogue(DLib.HANAKO, "<color=#DA4848>LET'S GOOOO!!!!!</color>");
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        GameManager.Instance.EndGame(2);
    }
}
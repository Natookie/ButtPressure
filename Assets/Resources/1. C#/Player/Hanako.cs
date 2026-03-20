using UnityEngine;
using System.Collections;

public class Hanako : MonoBehaviour
{
    [Header("TWEAKS")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float slowDownDistance = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask playerLayer;

    [Header("PATROL")]
    [SerializeField] private float patrolRange = 5f;
    [SerializeField] private float idleTime = 2f;

    private SpriteRenderer playerSr;
    private SpriteRenderer ghostSr;

    public bool canMove = false;
    public bool hasTeleported;

    private Transform playerTransform;
    private Vector3 lastKnownPlayerPosition;
    private bool playerDetected;
    private bool playerInLocker;
    private float patrolDirection = 1f;
    private float patrolTimer;
    private Vector3 patrolStartPosition;
    private BoxCollider2D boundary;
    private bool hasReachedLocker;

    void Start(){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) playerTransform = player.transform;
        PlayerMovement.Instance.hanako = this;

        ghostSr = this.gameObject.GetComponent<SpriteRenderer>();
        playerSr = player.GetComponent<SpriteRenderer>();
        
        patrolStartPosition = transform.position;
    }

    void Update(){
        if(!canMove || playerTransform == null) return;
        boundary = PlayerCam.Instance.GetBoundary();

        playerInLocker = !playerSr.enabled;
        DetectPlayer();
        
        float currentSpeed = CalculateSpeed();
        MoveTowardsTarget(currentSpeed);
        DrawDebugRay(currentSpeed);
    }

    void DetectPlayer(){
        if(playerInLocker){
            if(playerDetected && !hasReachedLocker) Debug.Log("<color=red>player hid but detected</color>");
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
        }
        else playerDetected = false;
    }

    float CalculateSpeed(){
        if(playerDetected) return runSpeed;
        else if(lastKnownPlayerPosition != Vector3.zero){
            float distanceToLastKnown = Mathf.Abs(transform.position.x - lastKnownPlayerPosition.x);
            
            if(distanceToLastKnown < slowDownDistance){
                if(distanceToLastKnown < 0.1f){
                    if(!hasReachedLocker && playerInLocker){
                        PlayerMovement.Instance.HideInLocker(lastKnownPlayerPosition, false);
                        hasReachedLocker = true;
                        StartCoroutine(CaughtSequence());
                    }
                    lastKnownPlayerPosition = Vector3.zero;
                    return 0f;
                }
                return Mathf.Lerp(0, walkSpeed, distanceToLastKnown / slowDownDistance);
            }
            return walkSpeed;
        }
        else return Patrol();
    }

    float Patrol(){
        patrolTimer += Time.deltaTime;
        
        if(patrolTimer >= idleTime){
            patrolDirection *= -1f;
            patrolTimer = 0f;
        }
        
        float newX = transform.position.x + (patrolDirection * walkSpeed * Time.deltaTime);
        
        if(boundary != null){
            newX = Mathf.Clamp(newX, 
                patrolStartPosition.x - patrolRange, 
                patrolStartPosition.x + patrolRange);
            
            newX = Mathf.Clamp(newX, 
                boundary.bounds.min.x + 0.5f, 
                boundary.bounds.max.x - 0.5f);
        }
        else{
            newX = Mathf.Clamp(newX, 
                patrolStartPosition.x - patrolRange, 
                patrolStartPosition.x + patrolRange);
        }
        
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
        
        if(boundary != null) newX = Mathf.Clamp(newX, boundary.bounds.min.x + 0.5f, boundary.bounds.max.x - 0.5f);
        
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void DrawDebugRay(float currentSpeed){
        if(playerTransform == null) return;
        
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distance = directionToPlayer.magnitude;
        float actualDistance = Mathf.Min(distance, detectionRange);
        
        Color rayColor = currentSpeed == runSpeed ? Color.red : Color.yellow;
        Debug.DrawRay(transform.position, directionToPlayer.normalized * actualDistance, rayColor);
    }

    public void TeleportTo(Vector3 position){
        transform.position = position;
        hasTeleported = true;
        
        if(ghostSr != null) ghostSr.flipX = position.x > transform.position.x;
    }

    bool isPlayerInLocker() => playerSr.enabled;

    IEnumerator CaughtSequence(){
        canMove = false;

        yield return new WaitForSeconds(0.1f);
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO,
            "Udin, Anterin gw ke Puri dong"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO,
            "Ekhem, maksud nya-|"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraShake.Instance.ShakeCamera(true);
        DialogueManager.Instance.SetDialogue(
            DLib.HANAKO,
            "<color=#DA4848>ISSHO NI AKUINATO O MIYOU!!!!!</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.HideDialogueUI();
        GameManager.Instance.EndGame(2);
    }
}
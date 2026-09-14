using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector] public UnityEvent ForcedMoveCompleted;

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator animator;

    [Header("MOVEMENT SETTINGS")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float forceMoveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.2f;

    [Header("Input")]
    public bool canInputMove = true;
    
    [Header("ANIMATION")]
    [SerializeField] private string walkingParameterName = "isWalking";

    [Header("AUDIO")]
    [SerializeField] private AudioClip walkSfxClip;

    public float horizontalDirection;
    private bool isForcedMoving = false;
    private Vector3 forcedMoveTargetPosition;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Start(){
        // Assertion check
        Debug.Assert(sr, "sr is missing");
    }

    void Update(){
        // Update sprite
        if(horizontalDirection > 0) sr.flipX = false;
        else if(horizontalDirection < 0) sr.flipX = true;
        // Update animation
        animator.SetBool(walkingParameterName, rb.linearVelocity != Vector2.zero);
        // Update audio
        if(rb.linearVelocity != Vector2.zero) AudioManager.Instance.PlaySFXLooping(walkSfxClip);
        else AudioManager.Instance.StopSFXLooping();
    }

    void FixedUpdate()
    {
        if (isForcedMoving)
        {
            // Do forced movement
            Vector2 direction = (forcedMoveTargetPosition - transform.position).normalized;
            Vector2 newVelocity = new Vector2(direction.x * forceMoveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = newVelocity;
            horizontalDirection = direction.x;
            float offset = Mathf.Abs(forcedMoveTargetPosition.x - transform.position.x);
            if(offset <= stoppingDistance){
                rb.linearVelocity = Vector2.zero;
                isForcedMoving = false;
                ForcedMoveCompleted?.Invoke();
            }
        }
        else if (canInputMove)
        {
            // Do input based movement
            horizontalDirection = 0;
            if (Keyboard.current.aKey.isPressed) horizontalDirection -= 1f;
            else if (Keyboard.current.dKey.isPressed) horizontalDirection += 1f;
            Vector2 newVelocity = new Vector2(horizontalDirection * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = newVelocity;
        }
    }
    #endregion

    // ====================================================================================================
    //                     Movement Functions
    // ====================================================================================================
    #region Movement
    public void StopMoving()
    {
        horizontalDirection = 0;
        rb.linearVelocity = Vector2.zero;
    }

    public void ForceMove(Vector3 targetPosition)
    {
        isForcedMoving = true;
        forcedMoveTargetPosition = targetPosition;
    }
    #endregion
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance {get; private set;}

    [Header("REFERENCES")]
    [SerializeField] private Rigidbody2D rb; 
    [SerializeField] private SpriteRenderer sr;

    [HideInInspector] public bool canMove;

    [Header("MOVEMENT SETTINGS")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float forceMoveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.1f;

    [Header("AUDIO")]
    [SerializeField] private string walkSfxName = "Footstep";
    
    [HideInInspector] public float horizontal;
    [HideInInspector] public bool hasReachedTarget;

    private bool isForcedMoving = false;
    private Transform forceMoveTarget;
    private BoxCollider2D boundaryCollider;
    private float playerHalfWidth;
    private float playerHalfHeight;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        if(sr != null){
            playerHalfWidth = sr.bounds.extents.x;
            playerHalfHeight = sr.bounds.extents.y;
        }
        
        ChangeBoundary();
    }

    void LateUpdate(){
        if(isForcedMoving){
            if(forceMoveTarget == null){
                isForcedMoving = false;
                return;
            }

            Vector2 direction = (forceMoveTarget.position - transform.position).normalized;
            Vector2 newVelocity = new Vector2(direction.x * forceMoveSpeed, rb.linearVelocity.y);
            
            if(boundaryCollider != null) newVelocity = ClampVelocityToBoundary(newVelocity);
            
            rb.linearVelocity = newVelocity;
            
            if(direction.x > 0) FaceLeft();
            else if(direction.x < 0) FaceRight();

            float xDifference = Mathf.Abs(forceMoveTarget.position.x - transform.position.x);
            if(xDifference <= stoppingDistance){
                rb.linearVelocity = Vector2.zero;
                isForcedMoving = false;
                hasReachedTarget = true;
            }
        }else{
            if(!GameManager.Instance.isInitialized || GameManager.Instance.isEnded) return;
            if(!sr.enabled) return;
            if(!canMove || GameManager.Instance.isInMiniGame) return;

            horizontal = (Keyboard.current.aKey.isPressed) ? -1f : (Keyboard.current.dKey.isPressed) ? 1f : 0f;
            Vector2 newVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
            
            if(boundaryCollider != null) newVelocity = ClampVelocityToBoundary(newVelocity);
            
            rb.linearVelocity = newVelocity;
            
            if(horizontal > 0) FaceLeft();
            else if(horizontal < 0) FaceRight();
        }

        if(boundaryCollider != null) transform.position = ClampPositionToBoundary(transform.position);

        UpdateAudio();
    }

    public void FaceRight() => sr.flipX = true;
    public void FaceLeft() => sr.flipX = false;

    public void ChangeBoundary(){
        if(PlayerCam.Instance == null) return;
        boundaryCollider = PlayerCam.Instance.GetBoundary();
    }

    Vector2 ClampVelocityToBoundary(Vector2 velocity){
        Bounds bounds = boundaryCollider.bounds;
        Vector3 currentPos = transform.position;
        Vector3 nextPos = currentPos + (Vector3)velocity * Time.deltaTime;
        
        if(nextPos.x - playerHalfWidth < bounds.min.x && velocity.x < 0)
            velocity.x = 0;
        else if(nextPos.x + playerHalfWidth > bounds.max.x && velocity.x > 0)
            velocity.x = 0;
        
        if(nextPos.y - playerHalfHeight < bounds.min.y && velocity.y < 0)
            velocity.y = 0;
        else if(nextPos.y + playerHalfHeight > bounds.max.y && velocity.y > 0)
            velocity.y = 0;
        
        return velocity;
    }

    Vector3 ClampPositionToBoundary(Vector3 position){
        Bounds bounds = boundaryCollider.bounds;
        
        float clampedX = Mathf.Clamp(position.x, bounds.min.x + playerHalfWidth, bounds.max.x - playerHalfWidth);
        float clampedY = Mathf.Clamp(position.y, bounds.min.y + playerHalfHeight, bounds.max.y - playerHalfHeight);
        
        return new Vector3(clampedX, clampedY, position.z);
    }

    public void ForceMove(Transform target){
        isForcedMoving = true;
        hasReachedTarget = false;
        forceMoveTarget = target;
    }

    public void SetPosition(Vector3 position){
        if(boundaryCollider != null) position = ClampPositionToBoundary(position);
        transform.position = position;
    }

    public void HideInLocker(Vector3 position, bool isInLocker){
        sr.enabled = !isInLocker;
        rb.linearVelocity = Vector2.zero;
        this.transform.position = new Vector3(position.x, transform.position.y, transform.position.z);
    }

    private void UpdateAudio()
    {
        if (rb.linearVelocity != Vector2.zero) AudioManager.Instance.PlaySFXLooping(walkSfxName);
        else AudioManager.Instance.StopSFXLooping();
    }
}
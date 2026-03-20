using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public static PlayerCam Instance {get; private set;}

    [Header("REFERENCES")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera thisCam;
    [SerializeField] private PlayerMovement pm;
    [Space(10)]
    [SerializeField] private GameObject linkedList;
    
    [Header("CAMERA SETTINGS")]
    [SerializeField] private float maxFollowDistance = 3f;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private Vector2 offset = Vector2.zero;
    [Space(10)]

    [Header("MOUSE TRACKING")]
    [SerializeField] private bool useMouseOffset;
    [SerializeField] private float mouseInfluence = 1f;
    [SerializeField] private bool invertX = false;

    [Header("BOUNDARY")]
    [SerializeField] private BoxCollider2D boundaryCollider;
    [SerializeField] private bool clampToBoundary = true;

    [Header("CUTSCENE")]
    [SerializeField] private float cutsceneSmoothSpeed = 5f;
    
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private Mouse mouse;
    private float cameraZDistance;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private float cameraHalfHeight;
    private float cameraHalfWidth;
    private float currentFollowDistance;
    private float followDistanceVelocity;
    
    private Transform cutsceneTarget;
    private bool isCutsceneMode = false;
    private Vector3 cutsceneOffset;
    private System.Action onCutsceneComplete;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        if(thisCam == null) thisCam = GetComponent<Camera>();
        GetCurrentBoundary();

        mouse = Mouse.current;
        cameraZDistance = -transform.position.z;
        
        CalculateCameraSize();
    }

    void CalculateCameraSize(){
        cameraHalfHeight = thisCam.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * thisCam.aspect;
    }

    void LateUpdate(){
        if(playerTransform == null || mouse == null || pm == null) return;
        if(!GameManager.Instance.isInitialized && !isCutsceneMode) return;

        if(isCutsceneMode && cutsceneTarget != null){
            targetPosition = cutsceneTarget.position + cutsceneOffset + (Vector3)offset;
            targetPosition.z = transform.position.z;
            
            if(clampToBoundary && boundaryCollider != null)
                targetPosition = ClampToBoundary(targetPosition);

            float cutsceneTime = cutsceneSmoothSpeed * Time.deltaTime;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, cutsceneTime, Mathf.Infinity, Time.deltaTime);
            
            return;
        }

        float horizontalOffset = 0f;

        if(pm.horizontal != 0){
            float targetDistance = maxFollowDistance;
            if(pm.horizontal != 0)
                currentFollowDistance = Mathf.SmoothDamp(currentFollowDistance, targetDistance, ref followDistanceVelocity, 1f);
            
            float direction = pm.horizontal > 0 ? 1f : -1f;
            horizontalOffset = direction * currentFollowDistance;
        }else{
            currentFollowDistance = Mathf.SmoothDamp(currentFollowDistance, 0f, ref followDistanceVelocity, 1f);
            
            if(useMouseOffset){
                Vector2 mouseScreenPos = mouse.position.ReadValue();
                Vector3 mouseScreenPoint = new Vector3(mouseScreenPos.x, mouseScreenPos.y, cameraZDistance);
                Vector3 mouseWorldPos = thisCam.ScreenToWorldPoint(mouseScreenPoint);
                mouseWorldPos.z = 0;

                float difference = mouseWorldPos.x - playerTransform.position.x;
                if(invertX) difference *= -1f;
                horizontalOffset = Mathf.Clamp(difference * mouseInfluence, -maxFollowDistance, maxFollowDistance);
            }
        }
        
        targetPosition = new Vector3(
            playerTransform.position.x + horizontalOffset + offset.x,
            playerTransform.position.y + offset.y,
            transform.position.z
        );

        if(clampToBoundary && boundaryCollider != null)
            targetPosition = ClampToBoundary(targetPosition);

        float followTime = smoothSpeed * Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followTime, Mathf.Infinity, Time.deltaTime);
    }

    Vector3 ClampToBoundary(Vector3 targetPos){
        Bounds bounds = boundaryCollider.bounds;
        
        float clampedX = Mathf.Clamp(targetPos.x, bounds.min.x + cameraHalfWidth, bounds.max.x - cameraHalfWidth);
        float clampedY = Mathf.Clamp(targetPos.y, bounds.min.y + cameraHalfHeight, bounds.max.y - cameraHalfHeight);
        
        return new Vector3(clampedX, clampedY, targetPos.z);
    }

    void GetCurrentBoundary(){
        if(linkedList == null) return;
        
        foreach(Transform child in linkedList.transform){
            if(child.gameObject.activeInHierarchy){
                BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
                if(collider != null){
                    boundaryCollider = collider;
                    break;
                }
            }
        }
    }

    public void FocusOnTarget(Transform target, Vector3 offset, System.Action onComplete = null){
        cutsceneTarget = target;
        cutsceneOffset = offset;
        isCutsceneMode = true;
        onCutsceneComplete = onComplete;
    }

    public void FocusOnTarget(Transform target, System.Action onComplete = null){
        FocusOnTarget(target, Vector3.zero, onComplete);
    }

    public void ReturnToPlayer(System.Action onComplete = null){
        cutsceneTarget = playerTransform;
        cutsceneOffset = Vector3.zero;
        onCutsceneComplete = onComplete;
    }

    public void EndCutsceneMode(){
        isCutsceneMode = false;
        cutsceneTarget = null;
        onCutsceneComplete?.Invoke();
        onCutsceneComplete = null;
    }

    public bool IsCutsceneMode() => isCutsceneMode;

    void OnDrawGizmosSelected(){
        if(playerTransform != null){
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(playerTransform.position, new Vector3(maxFollowDistance * 2, 5, 0));
            
            if(Application.isPlaying){
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetPosition, 0.2f);
            }
        }

        if(boundaryCollider != null){
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(boundaryCollider.bounds.center, boundaryCollider.bounds.size);
        }
    }

    public void SetConfiner(BoxCollider2D con){
        boundaryCollider = con;
        pm.ChangeBoundary();
    }
    public BoxCollider2D GetBoundary() => boundaryCollider;
}
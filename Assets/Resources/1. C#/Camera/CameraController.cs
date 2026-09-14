using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance {get; private set;}

    [Header("REFERENCES")]
    [SerializeField] private Transform followTarget;
    
    [Header("CAMERA SETTINGS")]
    [SerializeField] private float maxFollowDistance = 3f;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private Vector2 offset = Vector2.zero;
    public CameraBoundary cameraBoundary;

    [Header("MOUSE TRACKING")]
    public bool useMouseOffset = true;
    [SerializeField] private float mouseInfluence = 1f;
    [SerializeField] private bool invertX = false;
    
    private Camera thisCamera;
    private Mouse mouse;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private Transform defaultFollowTarget;

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
        thisCamera = GetComponent<Camera>();
        mouse = Mouse.current;
        defaultFollowTarget = followTarget;
    }

    void FixedUpdate(){
        // Check follow target
        if(!followTarget) return;
        // Apply mouse offset
        float mouseOffset = 0;
        if(useMouseOffset){
            Vector2 mouseScreenPos = mouse.position.ReadValue();
            Vector3 mouseScreenPoint = new Vector3(
                mouseScreenPos.x, mouseScreenPos.y, Math.Abs(thisCamera.transform.position.z)
            );
            Vector3 mouseWorldPos = thisCamera.ScreenToWorldPoint(mouseScreenPoint);
            float difference = mouseWorldPos.x - followTarget.position.x;
            if(invertX) difference *= -1f;
            mouseOffset = Mathf.Clamp(difference * mouseInfluence, -maxFollowDistance, maxFollowDistance);
        }
        // Set target position and clamp to boundary
        targetPosition = new Vector3(
            followTarget.position.x + mouseOffset + offset.x,
            followTarget.position.y + offset.y,
            transform.position.z
        );
        if (cameraBoundary) targetPosition = cameraBoundary.ClosestPointInBounds(targetPosition);
        // Apply smoothing
        float followTime = smoothSpeed * Time.deltaTime;
        transform.position = Vector3.SmoothDamp(
            transform.position, targetPosition, ref velocity, followTime, Mathf.Infinity, Time.deltaTime
        );
    }
    #endregion


    public void ChangeFollowTarget(Transform newFollowTarget)
    {
        followTarget = newFollowTarget;
    }

    public void ResetFollowTarget() => followTarget = defaultFollowTarget;
}
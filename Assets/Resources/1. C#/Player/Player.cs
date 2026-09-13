using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public UnityEvent ForcedMoveCompleted;
    
    public static Player Instance {get; private set;}

    [Header("REFERENCES")]
    [SerializeField] private PlayerMovement playerMovement;

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

    void Start(){
        // Assertion check
        Debug.Assert(playerMovement, "playerMovement is missing");
        // Connect event
        playerMovement.ForcedMoveCompleted.AddListener(()=>ForcedMoveCompleted?.Invoke());
    }
    #endregion

    // ====================================================================================================
    //                     Component Functions
    // ====================================================================================================
    #region Component
    public void ForceMove(Vector3 targetPosition) => playerMovement.ForceMove(targetPosition);
    #endregion
}

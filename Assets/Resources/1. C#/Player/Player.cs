using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public UnityEvent ForcedMoveCompleted;
    
    public static Player Instance {get; private set;}

    [Header("REFERENCES")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerInteraction playerInteraction;

    public bool EnableInput
    {
        set
        {
            playerMovement.canInputMove = value;
            playerMovement.StopMoving();
            playerInteraction.CanInteract = value;
        }
    }

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
        Debug.Assert(playerMovement, "playerMovement is missing");
        Debug.Assert(playerInteraction, "playerInteraction is missing");
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

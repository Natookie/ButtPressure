using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractableComponent))]
public class DoorPush : MonoBehaviour, IInteractable
{
    public UnityEvent DoorEntered;
    public UnityEvent DoorExited;
    public UnityEvent DoorLockedUsed;

    [Header("REFERENCES")]
    [SerializeField] private InteractableComponent interactableComponent;
    [SerializeField] private DoorPushMinigame doorPushMinigame;

    [Header("DOOR SETTINGS")]
    public string doorLocationName;
    public bool isLocked {private set; get;} = false;
    [SerializeField] private DoorPush linkedDoor;

    [Header("AUDIO")]
    [SerializeField] private AudioClip openSound;

    private bool hasPendingEvent = false;
    private bool hasPendingMinigame = false;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Start()
    {
        // Assertion check
        Debug.Assert(interactableComponent, "interactableComponent is missing");
        Debug.Assert(doorPushMinigame, "doorPushMinigame is missing");
        Debug.Assert(doorLocationName != "", "doorLocationName is empty");
        Debug.Assert(linkedDoor, "linkedDoor is empty");
        // Connect events
        doorPushMinigame.MinigameFinished.AddListener(OnMinigameFinished);
    }

    void OnEnable()
    {
        if (hasPendingEvent) DoorExited?.Invoke();
    }
    #endregion

    // ====================================================================================================
    //                     Door Functions
    // ====================================================================================================
    #region Door
    public void Interact(){
        if (isLocked) DoorLockedUsed?.Invoke();
        else if (linkedDoor)
        {
            hasPendingMinigame = true;
            doorPushMinigame.Interact();
        }
    }

    public void ToggleLock(bool isNowLocked)
    {
        isLocked = isNowLocked;
        interactableComponent.CanInteract = !isLocked;
    }

    private void TeleportToLinkedDoor(){   
        LocationController.Instance.ChangeLocation(linkedDoor.doorLocationName);  
        Player.Instance.transform.position = new Vector3(
            linkedDoor.transform.position.x,
            Player.Instance.transform.position.y,
            Player.Instance.transform.position.z
        );
        if (openSound) AudioManager.Instance.PlaySFX(openSound);
        DoorEntered?.Invoke();
        linkedDoor.AddPendingEvent();
    }

    public DoorPush GetLinkedDoor() => linkedDoor;
    public bool IsLocked() => isLocked;

    /// <summary>
    /// Internal use only!
    /// </summary>
    public bool AddPendingEvent() => hasPendingEvent = true;

    #endregion

    // ====================================================================================================
    //                     Event Functions
    // ====================================================================================================
    #region Event
    private void OnMinigameFinished()
    {
        if (hasPendingMinigame)
        {
            TeleportToLinkedDoor();
            hasPendingMinigame = false;
        }
    }
    #endregion
}

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(InteractableObject))]
public class Door : MonoBehaviour, IInteractable
{
    [Header("DOOR SETTINGS")]
    [SerializeField] private Door linkedDoor;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private AudioClip openSound;
    
    [Header("EVENT SETTINGS")]
    [SerializeField] private bool triggerEvent = false;
    [SerializeField] private UnityEvent onDoorUsed;
    
    private VisualCue visualCue;
    private AudioSource audioSource;

    void Start(){
        audioSource = GetComponent<AudioSource>();
        visualCue = VisualCue.Instance;
    }

    public void Interact(){
        if(isLocked){
            Debug.Log("Door is locked");
            return;
        }
        
        if(linkedDoor != null){
            TeleportToLinkedDoor();
            
            if(triggerEvent) onDoorUsed?.Invoke();
        }
    }

    void TeleportToLinkedDoor(){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player == null) return;
        
        Transform currentParent = transform.parent;
        Transform linkedParent = linkedDoor.transform.parent;
        
        bool hasDifferentParent = currentParent != linkedParent;
        
        if(hasDifferentParent){
            if(linkedParent != null && !linkedParent.gameObject.activeSelf)
                linkedParent.gameObject.SetActive(true);
            if(currentParent != null && currentParent.gameObject.activeSelf)
                currentParent.gameObject.SetActive(false);
            
            LocationUI.Instance.SetLocation(linkedParent.gameObject.name);
            PlayerCam.Instance.SetConfiner(linkedParent.gameObject.GetComponent<BoxCollider2D>());
        }else{
            if(currentParent != null) LocationUI.Instance.SetLocation(currentParent.gameObject.name);
        }
        
        Vector3 newPosition = linkedDoor.transform.position;
        newPosition.y = player.transform.position.y;
        newPosition.z = player.transform.position.z;
        player.transform.position = newPosition;
        
        InteractableObject linkedInteractable = linkedDoor.GetComponent<InteractableObject>();
        Debug.Log(linkedInteractable);
        visualCue.SetCurrentInteractable(linkedInteractable);
        
        if(openSound != null && audioSource != null) audioSource.PlayOneShot(openSound);
    }
}
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class Door : MonoBehaviour, IInteractable
{
    [Header("DOOR SETTINGS")]
    [SerializeField] private Door linkedDoor;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private bool preserveYPosition = true;
    
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
        
        if(linkedDoor != null) TeleportToLinkedDoor();
    }

    void TeleportToLinkedDoor(){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player == null) return;
        
        Transform currentParent = transform.parent;
        Transform linkedParent = linkedDoor.transform.parent;
        
        if(linkedParent != null && !linkedParent.gameObject.activeSelf)
            linkedParent.gameObject.SetActive(true);
        if(currentParent != null && currentParent.gameObject.activeSelf)
            currentParent.gameObject.SetActive(false);
        
        LocationUI.Instance.SetLocation(linkedParent.gameObject.name);
        PlayerCam.Instance.SetConfiner(linkedParent.gameObject.GetComponent<BoxCollider2D>());

        Vector3 newPosition = linkedDoor.transform.position;
        if(preserveYPosition) newPosition.y = player.transform.position.y;
        player.transform.position = newPosition;
        
        if(visualCue != null){
            InteractableObject linkedInteractable = linkedDoor.GetComponent<InteractableObject>();
            visualCue.SetCurrentInteractable(linkedInteractable);
        }

        if(openSound != null && audioSource != null) audioSource.PlayOneShot(openSound);
    }
}
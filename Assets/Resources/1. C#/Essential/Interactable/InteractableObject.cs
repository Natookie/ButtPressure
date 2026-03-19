using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("INTERACTION SETTINGS")]
    [SerializeField] private string interactionPrompt = "Press E to interact";
    [SerializeField] private bool oneTimeInteraction = false;
    [SerializeField] private float interactionRange = 2f;

    private VisualCue visualCue;
    private IInteractable[] interactableBehaviors;
    
    private bool hasInteracted = false;
    private Transform playerTransform;
    private bool playerInRange = false;

    public string Prompt => interactionPrompt;
    public bool CanInteract => !hasInteracted;

    void Start(){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) playerTransform = player.transform;
        if(visualCue == null) visualCue = VisualCue.Instance;
        
        interactableBehaviors = GetComponents<IInteractable>();
    }

    void Update(){
        if(playerTransform == null || visualCue == null || hasInteracted) return;

        float distance = Vector2.Distance(playerTransform.position, transform.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionRange;

        if(playerInRange && !wasInRange) visualCue.ShowPrompt(this);
        else if(!playerInRange && wasInRange) visualCue.HidePrompt();
    }

    public void Interact(){
        if(hasInteracted) return;
        
        foreach(var behavior in interactableBehaviors){
            behavior.Interact();
        }
        
        if(oneTimeInteraction){
            hasInteracted = true;
            visualCue.HidePrompt();
        }
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
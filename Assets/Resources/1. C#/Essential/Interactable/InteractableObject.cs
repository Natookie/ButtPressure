using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("INTERACTION SETTINGS")]
    [SerializeField] private string interactionPrompt = "Press E to interact";
    [SerializeField] private bool oneTimeInteraction = false;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private bool isMultiInteractable = false;

    [SerializeField] private bool firstHasDialogue = true;
    [SerializeField] private bool firstOneTime = true;
    [SerializeField] private bool secondHasDialogue = true;
    [SerializeField] private bool secondOneTime = true;

    private VisualCue visualCue;
    private IInteractable[] interactableBehaviors;
    private IMultiInteractable[] multiInteractableBehaviors;
    
    private bool hasInteracted = false;
    private Transform playerTransform;
    private bool playerInRange = false;
    private int interactionCount = 0;

    public string Prompt => interactionPrompt;
    public bool CanInteract => !hasInteracted;
    public bool IsMultiInteractable => isMultiInteractable;
    public bool FirstHasDialogue => firstHasDialogue;
    public bool SecondHasDialogue => secondHasDialogue;
    public bool FirstOneTime => firstOneTime;
    public bool SecondOneTime => secondOneTime;

    void Start(){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null) playerTransform = player.transform;
        if(visualCue == null) visualCue = VisualCue.Instance;
        
        interactableBehaviors = GetComponents<IInteractable>();
        multiInteractableBehaviors = GetComponents<IMultiInteractable>();
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

        interactionCount++;
        
        if(multiInteractableBehaviors.Length > 0){
            foreach(var behavior in multiInteractableBehaviors){
                if(interactionCount == 1) behavior.FirstInteraction();
                else behavior.SubsequentInteraction();
            }
        }
        
        foreach(var behavior in interactableBehaviors){
            behavior.Interact();
        }
        
        if(oneTimeInteraction){
            hasInteracted = true;
            visualCue.HidePrompt();
        }
    }

    public int GetInteractionCount() => interactionCount;
    public void SetPrompt(string value) => interactionPrompt = value;

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
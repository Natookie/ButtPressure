using UnityEngine;
using UnityEngine.InputSystem;
using Nova;

public class VisualCue : MonoBehaviour
{
    public static VisualCue Instance {get; private set;}

    [Header("REFERENCES")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Camera thisCam;

    [Header("UI REFERENCES")]
    [SerializeField] private UIBlock2D cueBlock;
    [SerializeField] private TextBlock interactionText;
    [SerializeField] private UIBlock2D iconBlock;
    
    [Header("UI SETTINGS")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float fadeSpeed = 5f;
    
    private Keyboard keyboard;
    private InteractableObject currentInteractable;
    private string currentText = "";
    private Color blockColor;
    private Color textColor;
    private Color iconColor;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        keyboard = Keyboard.current;
        
        if(cueBlock != null){
            blockColor = cueBlock.Color;
            blockColor.a = 0f;
            cueBlock.Color = blockColor;
        }
        
        if(interactionText != null){
            textColor = interactionText.Color;
            textColor.a = 0f;
            interactionText.Color = textColor;
            interactionText.Text = "";
        }
        
        if(iconBlock != null){
            iconColor = iconBlock.Color;
            iconColor.a = 0f;
            iconBlock.Color = iconColor;
        }
    }

    void LateUpdate(){
        bool shouldShowPrompt = currentInteractable != null && currentInteractable.enabled;
        
        if(shouldShowPrompt){
            string newText = currentInteractable.Prompt;
            
            if(newText != currentText){
                currentText = newText;
                if(interactionText != null) interactionText.Text = newText;
            }
            
            float targetAlpha = 1f;
            blockColor.a = Mathf.MoveTowards(blockColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            textColor.a = Mathf.MoveTowards(textColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            iconColor.a = Mathf.MoveTowards(iconColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            
            if(cueBlock != null) cueBlock.Color = blockColor;
            if(interactionText != null) interactionText.Color = textColor;
            if(iconBlock != null) iconBlock.Color = iconColor;
            
            if(keyboard != null && keyboard.eKey.wasPressedThisFrame && !DialogueManager.Instance.IsTyping){
                if(!currentInteractable.enabled) return;
                bool shouldStopMovement = false;
                
                if(currentInteractable.IsMultiInteractable){
                    int count = currentInteractable.GetInteractionCount();
                    if(count == 0) shouldStopMovement = currentInteractable.FirstHasDialogue;
                    else if(count >= 1) shouldStopMovement = currentInteractable.SecondHasDialogue;
                }else shouldStopMovement = currentInteractable.FirstHasDialogue;
                
                if(shouldStopMovement){
                    PlayerMovement.Instance.canMove = false;
                    PlayerMovement.Instance.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
                }
                
                currentInteractable.Interact();
            }
        }
        else{
            float targetAlpha = 0f;
            blockColor.a = Mathf.MoveTowards(blockColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            textColor.a = Mathf.MoveTowards(textColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            iconColor.a = Mathf.MoveTowards(iconColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            
            if(cueBlock != null) cueBlock.Color = blockColor;
            if(interactionText != null) interactionText.Color = textColor;
            if(iconBlock != null) iconBlock.Color = iconColor;
            
            if(blockColor.a <= 0.01f && textColor.a <= 0.01f && iconColor.a <= 0.01f){
                if(interactionText != null) interactionText.Text = "";
                currentText = "";
            }
        }
    }

    public void ShowPrompt(InteractableObject interactable) => currentInteractable = interactable;
    public void HidePrompt() => currentInteractable = null;
    public void SetCurrentInteractable(InteractableObject interactable) => currentInteractable = interactable;
}
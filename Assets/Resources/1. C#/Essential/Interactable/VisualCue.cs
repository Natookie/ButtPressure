using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class VisualCue : MonoBehaviour
{
    public static VisualCue Instance {get; private set;}

    [Header("REFERENCES")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Camera thisCam;

    [Header("UI REFERENCES")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private TextMeshProUGUI interactionText;
    
    [Header("UI SETTINGS")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float minWidth = 100f;
    [SerializeField] private float maxWidth = 400f;
    [SerializeField] private float padding = 20f;
    
    private CanvasGroup canvasGroup;
    private Keyboard keyboard;
    private InteractableObject currentInteractable;
    private RectTransform canvasRect;
    private RectTransform textRect;
    private string currentText = "";

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        keyboard = Keyboard.current;
        
        uiCanvas.renderMode = RenderMode.WorldSpace;
        uiCanvas.worldCamera = thisCam;
        
        canvasRect = uiCanvas.GetComponent<RectTransform>();
        textRect = interactionText.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(400, 100);
        canvasRect.localScale = Vector3.one * 0.01f;
        
        canvasGroup = uiCanvas.GetComponent<CanvasGroup>();
        if(canvasGroup == null) canvasGroup = uiCanvas.gameObject.AddComponent<CanvasGroup>();
            
        canvasGroup.alpha = 0f;
        interactionText.text = "";
    }

    void Update(){
        if(playerTransform == null || thisCam == null) return;

        if(currentInteractable != null){
            float dynamicOffsetX = Mathf.Abs(offset.x);
            if(playerSpriteRenderer != null){
                dynamicOffsetX = (playerSpriteRenderer.flipX) ? -dynamicOffsetX : dynamicOffsetX;
                interactionText.alignment = (playerSpriteRenderer.flipX) ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
            }
            
            transform.position = playerTransform.position + new Vector3(dynamicOffsetX, offset.y, offset.z);
            
            string newText = currentInteractable.Prompt;
            
            if(newText != currentText){
                currentText = newText;
                UpdateTextBoxSize(newText);
            }
            
            interactionText.text = newText;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
            
            if(keyboard != null && keyboard.eKey.wasPressedThisFrame && !DialogueManager.Instance.IsTyping){
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
        }else{
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
            if(canvasGroup.alpha <= 0.01f){
                interactionText.text = "";
                currentText = "";
            }
        }
    }
    
    void UpdateTextBoxSize(string text){
        if(string.IsNullOrEmpty(text)){
            textRect.sizeDelta = new Vector2(minWidth, textRect.sizeDelta.y);
            return;
        }
        
        interactionText.text = text;
        interactionText.ForceMeshUpdate();
        
        Bounds bounds = interactionText.bounds;
        
        float textWidth = bounds.size.x;
        float textHeight = bounds.size.y;
        
        float newWidth = textWidth + padding;
        float newHeight = textHeight + padding;
        
        newWidth = Mathf.Clamp(newWidth, minWidth, maxWidth);
        textRect.sizeDelta = new Vector2(newWidth, newHeight);
    }

    public void ShowPrompt(InteractableObject interactable) => currentInteractable = interactable;
    public void HidePrompt() => currentInteractable = null;
    public void SetCurrentInteractable(InteractableObject interactable) => currentInteractable = interactable;
}
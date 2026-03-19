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
    
    private CanvasGroup canvasGroup;
    private Keyboard keyboard;
    private InteractableObject currentInteractable;
    private RectTransform canvasRect;

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
            if(playerSpriteRenderer != null && playerSpriteRenderer.flipX)
                dynamicOffsetX = -dynamicOffsetX;
            
            transform.position = playerTransform.position + new Vector3(dynamicOffsetX, offset.y, offset.z);
            
            interactionText.text = currentInteractable.Prompt;
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
            
            if(keyboard != null && keyboard.eKey.wasPressedThisFrame) currentInteractable.Interact();
        }else{
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
            if(canvasGroup.alpha <= 0.01f) interactionText.text = "";
        }
    }

    public void ShowPrompt(InteractableObject interactable) => currentInteractable = interactable;
    public void HidePrompt() => currentInteractable = null;
    public void SetCurrentInteractable(InteractableObject interactable) => currentInteractable = interactable;
}
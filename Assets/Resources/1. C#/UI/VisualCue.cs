using UnityEngine;
using Nova;

public class VisualCue : MonoBehaviour
{
    public static VisualCue Instance {get; private set;}

    [Header("UI REFERENCES")]
    [SerializeField] private UIBlock2D cueBlock;
    [SerializeField] private TextBlock interactionText;
    [SerializeField] private UIBlock2D iconBlock;
    
    [Header("UI SETTINGS")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float fadeSpeed = 5f;
    
    private bool isShowing = false;
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

    void Update(){
        if (isShowing)
        {
            float targetAlpha = 1f;
            blockColor.a = Mathf.MoveTowards(blockColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            textColor.a = Mathf.MoveTowards(textColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            iconColor.a = Mathf.MoveTowards(iconColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            if(cueBlock != null) cueBlock.Color = blockColor;
            if(interactionText != null) interactionText.Color = textColor;
            if(iconBlock != null) iconBlock.Color = iconColor;
        }
        else
        {
            float targetAlpha = 0f;
            blockColor.a = Mathf.MoveTowards(blockColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            textColor.a = Mathf.MoveTowards(textColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            iconColor.a = Mathf.MoveTowards(iconColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            if(cueBlock != null) cueBlock.Color = blockColor;
            if(interactionText != null) interactionText.Color = textColor;
            if(iconBlock != null) iconBlock.Color = iconColor;
        }
    }

    public void ShowPrompt(InteractableComponent interactable)
    {
        interactionText.Text = interactable.Prompt;
        isShowing = true;
    }

    public void HidePrompt() {isShowing = false;}
}
using UnityEngine;
using System.Collections;
using Nova;
using UnityEngine.InputSystem;

public class DoorPush : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D miniGameBlock;
    [SerializeField] private UIBlock2D hitCircle;
    [SerializeField] private Door doorToUnlock;
    [SerializeField] private TextBlock infoText;
    
    [Header("BOUNDARIES")]
    [SerializeField] private float minX = -700f;
    [SerializeField] private float maxX = 700f;
    [SerializeField] private float minY = -220f;
    [SerializeField] private float maxY = 220f;
    
    [Header("COLORS")]
    [SerializeField] private Color hoverColor = new Color(1f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color normalBorderColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color pressColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color missColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Red for miss
    
    [Header("TWEAKS")]
    [SerializeField] private bool needIntroduction;
    [SerializeField] private int minTargetHits = 10;
    [SerializeField] private int maxTargetHits = 15;
    [SerializeField] private float pressFeedbackDuration = 0.1f;
    [SerializeField] private int missPenalty = 1;

    [Header("DEBUG")]
    [SerializeField] private bool skipMinigame;

    private InteractableObject io;
    
    private bool hasIntroduced;
    private int currentHit;
    private int targetHit;
    private bool isMinigameActive = false;
    private Keyboard keyboard;
    private Mouse mouse;
    private Coroutine pressFeedbackCoroutine;
    private Coroutine missFeedbackCoroutine;

    void Start(){
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        
        if(miniGameBlock != null) miniGameBlock.gameObject.SetActive(false);
        
        if(infoText != null) infoText.Text = "";
            
        if(hitCircle != null){
            hitCircle.AddGestureHandler<Gesture.OnHover>(OnCircleHover);
            hitCircle.AddGestureHandler<Gesture.OnUnhover>(OnCircleUnhover);
            hitCircle.AddGestureHandler<Gesture.OnPress>(OnCirclePress);
        }

        io = GetComponent<InteractableObject>();
        if(needIntroduction) io.SetPrompt("Open Cafetaria Door");
    }

    void Update(){
        if(isMinigameActive && mouse != null && mouse.leftButton.wasPressedThisFrame){
            if(!wasCirclePressedThisFrame) MissClick();
            wasCirclePressedThisFrame = false;
        }
    }
    
    private bool wasCirclePressedThisFrame = false;
    
    void UpdateInfoText(){
        if(infoText != null) infoText.Text = $"Hit the circle [{currentHit}/{targetHit}]";
    }
    
    void MissClick(){
        currentHit -= missPenalty;
        
        if(currentHit < 0) currentHit = 0;
        
        if(missFeedbackCoroutine != null) StopCoroutine(missFeedbackCoroutine);
        missFeedbackCoroutine = StartCoroutine(MissFeedback());

        UpdateInfoText();
    }
    
    IEnumerator MissFeedback(){
        Color originalColor = hitCircle.Border.Color;
        
        hitCircle.Border.Color = missColor;
        
        yield return new WaitForSeconds(pressFeedbackDuration);
        
        if(isMinigameActive) hitCircle.Border.Color = normalBorderColor;
        else hitCircle.Border.Color = originalColor;
            
        missFeedbackCoroutine = null;
    }

    public void Interact(){
        if(needIntroduction && !hasIntroduced){
            StartCoroutine(IntroduceDialogue());
            return;
        }
        
        StartMinigame();
    }
    
    void StartMinigame(){
        if(skipMinigame){
            EndMinigame();
            return;
        }

        currentHit = 0;
        targetHit = Random.Range(minTargetHits, maxTargetHits + 1);
        isMinigameActive = true;
        
        miniGameBlock.gameObject.SetActive(true);
        SetRandomCirclePosition();
        UpdateInfoText();
        
        if(PlayerMovement.Instance != null) PlayerMovement.Instance.canMove = false;
    }
    
    void EndMinigame(){
        isMinigameActive = false;
        miniGameBlock.gameObject.SetActive(false);
        
        if(PlayerMovement.Instance != null) PlayerMovement.Instance.canMove = true;
        if(doorToUnlock != null) doorToUnlock.CompleteMinigame();
        io.SetPrompt("Open Cafetaria Door");
        
        if(infoText != null) infoText.Text = "";
    }
    
    void SetRandomCirclePosition(){
        if(hitCircle == null) return;
        
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        
        hitCircle.Position.Value = new Vector3(randomX, randomY, 0);
    }
    
    void OnCircleHover(Gesture.OnHover evt){
        if(!isMinigameActive) return;
        hitCircle.Border.Color = hoverColor;
    }
    
    void OnCircleUnhover(Gesture.OnUnhover evt){
        if(!isMinigameActive) return;
        hitCircle.Border.Color = normalBorderColor;
    }
    
    void OnCirclePress(Gesture.OnPress evt){
        if(!isMinigameActive) return;
        
        wasCirclePressedThisFrame = true;
        currentHit++;
        
        if(pressFeedbackCoroutine != null) StopCoroutine(pressFeedbackCoroutine);
        pressFeedbackCoroutine = StartCoroutine(PressFeedback());
        
        UpdateInfoText();
        
        if(currentHit >= targetHit){
            EndMinigame();
            return;
        }
        
        SetRandomCirclePosition();
    }
    
    IEnumerator PressFeedback(){
        Color originalColor = hitCircle.Border.Color;
        
        hitCircle.Border.Color = pressColor;
        
        yield return new WaitForSeconds(pressFeedbackDuration);
        
        if(isMinigameActive) hitCircle.Border.Color = normalBorderColor;
        else hitCircle.Border.Color = originalColor;
            
        pressFeedbackCoroutine = null;
    }
    
    public IEnumerator IntroduceDialogue(){
        yield return new WaitForSeconds(0.1f);
        
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        PlayerMovement.Instance.canMove = false;
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "The cafeteria door is really heavy.\n~I need to push harder."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
        
        PlayerMovement.Instance.canMove = true;
        io.SetPrompt("Push Cafetaria Door");

        hasIntroduced = true;
        StartMinigame();
    }
    
    public void ResetMinigame(){
        currentHit = 0;
        isMinigameActive = false;
        if(miniGameBlock != null) miniGameBlock.gameObject.SetActive(false);
        
        if(PlayerMovement.Instance != null)
            PlayerMovement.Instance.canMove = true;
        
        if(infoText != null)
            infoText.Text = "";
    }
    
    public void SetHasIntroduced(bool introduced) => hasIntroduced = introduced;
    public void StartMinigameFromDoor() => Interact();
}
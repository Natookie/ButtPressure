using UnityEngine;
using System.Collections;
using Nova;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class DoorPushMinigame : MonoBehaviour
{
    public UnityEvent MinigameFinished;

    [Header("REFERENCES")]
    [SerializeField] private UIBlock2D miniGameBlock;
    [SerializeField] private UIBlock2D hitCircle;
    [SerializeField] private TextBlock infoText;
    [SerializeField] private InteractableComponent interactableComponent;
    
    [Header("BOUNDARIES")]
    [SerializeField] private float minX = -700f;
    [SerializeField] private float maxX = 700f;
    [SerializeField] private float minY = -220f;
    [SerializeField] private float maxY = 220f;
    
    [Header("COLORS")]
    [SerializeField] private Color hoverColor = new Color(1f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color normalBorderColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color pressColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color missColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    
    [Header("TWEAKS")]
    [SerializeField] private bool needIntroduction;
    [SerializeField] private int minTargetHits = 10;
    [SerializeField] private int maxTargetHits = 15;
    [SerializeField] private float pressFeedbackDuration = 0.1f;
    [SerializeField] private int missPenalty = 1;

    [Header("AUDIO")]
    [SerializeField] private string successSfxName = "Push Success";
    [SerializeField] private string missSfxName = "Push Miss";
    
    private bool hasIntroduced;
    private int currentHit;
    private int targetHit;
    private bool isMinigameActive = false;
    private bool hasSavedProgress = false;
    private Keyboard keyboard;
    private Mouse mouse;
    private Coroutine pressFeedbackCoroutine;
    private Coroutine missFeedbackCoroutine;

    void Start()
    {
        Debug.Assert(miniGameBlock, "miniGameBlock is missing");
        Debug.Assert(infoText, "infoText is missing");
        Debug.Assert(hitCircle, "hitCircle is missing");
        Debug.Assert(interactableComponent, "interactableComponent is missing");

        keyboard = Keyboard.current;
        mouse = Mouse.current;
        
        miniGameBlock.gameObject.SetActive(false);
        infoText.Text = "";
        hitCircle.AddGestureHandler<Gesture.OnHover>(OnCircleHover);
        hitCircle.AddGestureHandler<Gesture.OnUnhover>(OnCircleUnhover);
        hitCircle.AddGestureHandler<Gesture.OnPress>(OnCirclePress);

        if(needIntroduction) interactableComponent.SetPrompt("Open Cafetaria Door");
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

        AudioManager.Instance.PlaySFX(missSfxName);
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
        if(isMinigameActive){
            CloseAndSaveProgress();
            return;
        }
        
        if(hasSavedProgress && currentHit > 0 && currentHit < targetHit){
            ResumeMinigame();
            return;
        }
        
        if(needIntroduction && !hasIntroduced){
            StartCoroutine(IntroduceDialogue());
            return;
        }
        
        StartMinigame();
    }
    
    void StartMinigame(){
        if (GameDebug.Instance.skipMinigame)
        {
            EndMinigame();
            return;
        }

        currentHit = 0;
        targetHit = Random.Range(minTargetHits, maxTargetHits + 1);
        isMinigameActive = true;
        hasSavedProgress = false;
        
        miniGameBlock.gameObject.SetActive(true);
        SetRandomCirclePosition();
        UpdateInfoText();
        
        Player.Instance.EnableInput = false;
        CameraController.Instance.useMouseOffset = false;
    }
    
    void EndMinigame(){
        isMinigameActive = false;
        hasSavedProgress = false;
        miniGameBlock.gameObject.SetActive(false);

        Player.Instance.EnableInput = true;
        CameraController.Instance.useMouseOffset = true;
        MinigameFinished?.Invoke();
        interactableComponent.SetPrompt("Open Cafetaria Door");
        
        if(infoText != null) infoText.Text = "";
        
        currentHit = 0;
        targetHit = 0;
    }
    
    public void CloseAndSaveProgress(){
        hasSavedProgress = true;
        isMinigameActive = false;
        miniGameBlock.gameObject.SetActive(false);
        
        if(interactableComponent != null) interactableComponent.enabled = true;
        Player.Instance.EnableInput = true;
        CameraController.Instance.useMouseOffset = true;
        
        Debug.Log($"Minigame saved! Progress: {currentHit}/{targetHit}");
    }
    
    public void ResumeMinigame(){
        if(isMinigameActive) return;
        if(!hasSavedProgress) return;
        if(currentHit >= targetHit) return;
        
        isMinigameActive = true;
        miniGameBlock.gameObject.SetActive(true);
        SetRandomCirclePosition();
        UpdateInfoText();
        
        Player.Instance.EnableInput = false;
        CameraController.Instance.useMouseOffset = false;
        
        Debug.Log($"Minigame resumed! Progress: {currentHit}/{targetHit}");
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

        AudioManager.Instance.PlaySFX(successSfxName);
        
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
        
        Player.Instance.EnableInput = false;
        CameraController.Instance.useMouseOffset = false;
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "The cafeteria door is really heavy.\n~I need to push harder."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
        
        Player.Instance.EnableInput = true;
        CameraController.Instance.useMouseOffset = true;
        interactableComponent.SetPrompt("Push Cafetaria Door");

        hasIntroduced = true;
        StartMinigame();
    }
    
    public void ResetMinigame(){
        currentHit = 0;
        targetHit = 0;
        isMinigameActive = false;
        hasSavedProgress = false;
        if(miniGameBlock != null) miniGameBlock.gameObject.SetActive(false);
        
        interactableComponent.enabled = true;
        
        Player.Instance.EnableInput = true;
        CameraController.Instance.useMouseOffset = true;
        
        if(infoText != null)
            infoText.Text = "";
    }
    
    public void SetHasIntroduced(bool introduced) => hasIntroduced = introduced;
    public void StartMinigameFromDoor() => Interact();
    
    public bool IsMinigameActive() => isMinigameActive;
    public bool HasSavedProgress() => hasSavedProgress;
    public int GetCurrentHits() => currentHit;
    public int GetTargetHits() => targetHit;
    
    public void LoadProgress(int savedHits, int savedTarget){
        currentHit = savedHits;
        targetHit = savedTarget;
        if(targetHit <= 0) targetHit = Random.Range(minTargetHits, maxTargetHits + 1);
        hasSavedProgress = true;
    }
}
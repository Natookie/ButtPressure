using UnityEngine;
using System.Collections;
using Nova;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InteractableObject))]
public class Obstacle : MonoBehaviour, IMultiInteractable
{
    [Header("UI REFERENCES")]
    [SerializeField] private UIBlock2D innerCircleBlock;
    [SerializeField] private GameObject miniGame;
    [SerializeField] private ObstacleUI oui;
    
    [Header("MINIGAME SETTINGS")]
    [SerializeField] private float initialRotationSpeed = 90f;
    [SerializeField] private float initialFillAngle = 90f;
    [Space(10)]
    [SerializeField] private float speedIncreasePerSuccess = 20f;
    [SerializeField] private float maxRotationSpeed = 180f;
    [Space(10)]
    [SerializeField] private float minFillAngle = 15f;
    [SerializeField] private float angleReduction = 15f;
    [SerializeField] private int requiredCorrectPresses = 12;
    [Space(10)]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private Color neutralColor = Color.white;
    
    private bool firstExecuted = false;
    private bool minigameActive = false;
    private bool minigameCompleted = false;
    private int correctPressCount = 0;
    private float currentFillAngle;
    private float currentRotationSpeed;
    private float currentRotation;
    private bool waitingForInput = false;
    private Keyboard keyboard;
    
    void Start(){
        keyboard = Keyboard.current;
        if(miniGame != null) miniGame.SetActive(false);
    }
    
    void Update(){
        if(!minigameActive) return;
        
        currentRotation += currentRotationSpeed * Time.deltaTime;
        if(currentRotation >= 360f) currentRotation = -360f + (currentRotation - 360f);
        else if(currentRotation <= -360f) currentRotation = 360f + (currentRotation + 360f);
        
        innerCircleBlock.RadialFill.Rotation = currentRotation;
        
        if(keyboard != null && keyboard.spaceKey.wasPressedThisFrame && !waitingForInput)
            StartCoroutine(CheckInput());
    }
    
    IEnumerator CheckInput(){
        waitingForInput = true;
        
        float normalizedAngle = currentRotation;
        if(normalizedAngle < 0) normalizedAngle += 360f;
        
        bool isCorrect = (normalizedAngle <= currentFillAngle) || (normalizedAngle >= (360f - currentFillAngle));
        
        if(isCorrect){
            correctPressCount++;
            innerCircleBlock.Color = correctColor;
            
            currentFillAngle = Mathf.Max(currentFillAngle - angleReduction, minFillAngle);
            currentRotationSpeed = Mathf.Min(currentRotationSpeed + speedIncreasePerSuccess, maxRotationSpeed);
            innerCircleBlock.RadialFill.FillAngle = currentFillAngle;
            
            oui.AddProgress(correctPressCount);
            
            if(correctPressCount % 3 == 0 && correctPressCount > 0) oui.AddTableCount();
            
            if(correctPressCount >= requiredCorrectPresses){
                yield return new WaitForSeconds(0.3f);
                CompleteMinigame();
            }else SetRandomRotation();
        }
        else{
            innerCircleBlock.Color = incorrectColor;
            
            int completedTables = correctPressCount / 3;
            int minPressCount = completedTables * 3;
            
            correctPressCount = Mathf.Max(correctPressCount - 1, minPressCount);
            oui.AddProgress(correctPressCount);
        }
        
        yield return new WaitForSeconds(0.2f);
        innerCircleBlock.Color = neutralColor;
        waitingForInput = false;
    }
    
    void SetRandomRotation(){
        float randomRotation = Random.Range(-360f, 360f);
        currentRotation = randomRotation;
        innerCircleBlock.RadialFill.Rotation = currentRotation;
    }
    
    void CompleteMinigame(){
        minigameActive = false;
        minigameCompleted = true;
        innerCircleBlock.Color = correctColor;
        
        if(miniGame != null) miniGame.SetActive(false);
        if(GameManager.Instance != null) GameManager.Instance.isInMiniGame = false;
        
        StartCoroutine(CompleteDialogue());
    }
    
    IEnumerator CompleteDialogue(){
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I did it. I should stop talking and actually run now."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "But afaik i only have 1 animation for walking~\nAnd i am still wasting my time talking to you."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
        
        InteractableObject interactable = GetComponent<InteractableObject>();
        if(interactable != null) interactable.enabled = false;
    }
    
    public void FirstInteraction(){
        if(firstExecuted) return;
        StartCoroutine(First());
    }
    
    public void SubsequentInteraction(){
        if(minigameCompleted) return;
        if(!firstExecuted){
            StartCoroutine(First());
            return;
        }
        
        if(GameManager.Instance != null && GameManager.Instance.isInMiniGame) CloseMinigame();
        else OpenMinigame();
    }
    
    void OpenMinigame(){
        if(miniGame != null) miniGame.SetActive(true);
        if(GameManager.Instance != null) GameManager.Instance.isInMiniGame = true;
        
        StartMinigame();
    }
    
    void CloseMinigame(){
        if(miniGame != null) miniGame.SetActive(false);
        if(GameManager.Instance != null) GameManager.Instance.isInMiniGame = false;
        
        minigameActive = false;
        waitingForInput = false;
    }
    
    void StartMinigame(){
        minigameActive = true;
        
        SetRandomRotation();
        
        innerCircleBlock.RadialFill.FillAngle = currentFillAngle;
        innerCircleBlock.Color = neutralColor;
        
        oui.AddProgress(correctPressCount);
        int completedTables = correctPressCount / 3;
        for(int i = 0; i < completedTables; i++) oui.AddTableCount();
    }
    
    IEnumerator First(){
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "THE STAIRWAY IS BLOCKED! I NEED TO PULL IT"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
        firstExecuted = true;
        GetComponent<InteractableObject>().SetPrompt("Pull cart");
        
        currentFillAngle = initialFillAngle;
        currentRotationSpeed = initialRotationSpeed;
        correctPressCount = 0;
        
        SetRandomRotation();
    }
}
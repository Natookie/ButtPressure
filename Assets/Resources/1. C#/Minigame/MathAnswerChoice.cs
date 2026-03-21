using System;
using System.Collections;
using Nova;
using UnityEngine;

public class MathAnswerChoice : MonoBehaviour
{
    public event EventHandler<OnAnswerChoicePickedEventArgs> OnAnswerChoicePicked;
    public class OnAnswerChoicePickedEventArgs : EventArgs{
        public int answerNumber;
    }

    [Header("Component and Object")]
    [SerializeField] private TextBlock answerText;
    [SerializeField] private UIBlock2D answerBlock;
    
    [Header("Color Settings")]
    [SerializeField] private Color correctColor = new Color(0.2f, 0.8f, 0.2f, 1f); // Green
    [SerializeField] private Color wrongColor = new Color(0.8f, 0.2f, 0.2f, 1f); // Red
    [SerializeField] private Color defaultColor = new Color(1f, 1f, 1f, 1f); // White
    [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray
    [SerializeField] private float colorLerpDuration = 0.3f;

    private Interactable interactable;

    private int currentAnswerNumber = 0;
    private Coroutine colorCoroutine;
    private bool isInteractable = true;

    // ====================================================================================================
    //                     Start Functions
    // ====================================================================================================
    #region Start
    public void Start(){
        // Assertion Check
        Debug.Assert(answerText, "answerText is missing");
        Debug.Assert(answerBlock, "answerBlock is missing - assign the UIBlock2D component");
        
        interactable = GetComponent<Interactable>();

        SetupGestureHandlers();
        answerBlock.Color = defaultColor;
    }

    void Update(){
        interactable.enabled = isInteractable;
    }
    
    void SetupGestureHandlers(){
        answerBlock.AddGestureHandler<Gesture.OnPress>(OnPress);
        
        answerBlock.AddGestureHandler<Gesture.OnHover>(OnHover);
        answerBlock.AddGestureHandler<Gesture.OnUnhover>(OnUnhover);
    }
    
    void OnPress(Gesture.OnPress evt){
        if(isInteractable) AnswerChoicePicked();
    }
    
    void OnHover(Gesture.OnHover evt){
        if(isInteractable && colorCoroutine == null){
            Color hoverColor = new Color(defaultColor.r * 0.85f, defaultColor.g * 0.85f, defaultColor.b * 0.85f, defaultColor.a);
            answerBlock.Color = hoverColor;
        }
    }
    
    void OnUnhover(Gesture.OnUnhover evt){
        if(isInteractable && colorCoroutine == null){
            if(answerBlock.Color != correctColor && answerBlock.Color != wrongColor)
                answerBlock.Color = defaultColor;
        }
    }
    #endregion

    // ====================================================================================================
    //                     Answer Choice Functions
    // ====================================================================================================
    #region Answer Choice
    public void SetAnswerChoice(int number){
        currentAnswerNumber = number;
        answerText.Text = number.ToString();
        ResetAnswerColor();
    }
    
    public void SetInteractable(bool interactableState){
        isInteractable = interactableState;
        
        if(!interactableState){
            if(colorCoroutine == null) answerBlock.Color = disabledColor;
        }
        else{
            if(colorCoroutine == null && answerBlock.Color != correctColor && answerBlock.Color != wrongColor)
                answerBlock.Color = defaultColor;
        }
    }
    
    public void AnswerChoicePicked(){
        OnAnswerChoicePicked?.Invoke(
            this,
            new OnAnswerChoicePickedEventArgs{answerNumber = currentAnswerNumber}
        );
    }
    
    public void AnimateAnswerFeedback(bool isCorrect){
        if(colorCoroutine != null){
            StopCoroutine(colorCoroutine);
            colorCoroutine = null;
        }
        
        Color targetColor = isCorrect ? correctColor : wrongColor;
        colorCoroutine = StartCoroutine(LerpColor(targetColor, colorLerpDuration));
    }
    
    public void ResetAnswerColor(){
        if(colorCoroutine != null){
            StopCoroutine(colorCoroutine);
            colorCoroutine = null;
        }
        answerBlock.Color = defaultColor;
    }
    
    IEnumerator LerpColor(Color targetColor, float duration){
        Color startColor = answerBlock.Color;
        float elapsed = 0f;
        
        while (elapsed < duration){
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            answerBlock.Color = Color.Lerp(startColor, targetColor, smoothT);
            yield return null;
        }
        
        answerBlock.Color = targetColor;
        colorCoroutine = null;
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using Nova;
using UnityEngine;
using UnityEngine.Events;

public class MathMinigameController : MonoBehaviour
{
    public UnityEvent OnMinigameFinished;

    [Header("Component and Object")]
    [SerializeField] private GameObject content;
    [SerializeField] private UIBlock contentBlock;
    [SerializeField] private TextBlock questionText;
    [SerializeField] private TextBlock questionCount;
    [SerializeField] private List<MathAnswerChoice> answerChoiceList;
    [Header("Minigame Settings")]
    [SerializeField] private int minQuestionAmount = 3;
    [SerializeField] private int maxQuestionAmount = 3;
    [Tooltip("The minimum number a question can create")]
    [SerializeField] private int minQuestionVariance = 0;
    [Tooltip("The maximum number a question can create")]
    [SerializeField] private int maxQuestionVariance = 9;
    [Header("Wrong Answer Settings")]
    [SerializeField] private float wrongAnswerDelay = 1.5f;
    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float bounceAmount = 20f;
    [SerializeField] private float bounceDuration = 0.2f;
    [Header("Audio")]
    [SerializeField] private AudioClip correctAnswerSfxClip;
    [Header("Debug")]
    [Tooltip("Start the minigame when played, default is false")]
    [SerializeField] private bool startMinigameOnRun = false;

    [Header("REFERENCE")]
    [SerializeField] private VendingMachine vendingMachine;
    [SerializeField] private InteractableObject richKid;

    private string fuckeryFuckText;

    // Gameflow
    private int questionAmount = 0;
    private int answeredAmount = 0;
    private int correctAnswerAmount = 0;
    private int wrongAnswerAmount = 0;
    // Math
    private int correctAnswerNumber = 0;
    // State management
    private bool isWaitingForWrongAnswer = false;
    private Coroutine wrongAnswerDelayCoroutine;
    
    // Animation
    private Vector3 originalPosition;
    private Vector3 bottomPosition;
    private Coroutine animationCoroutine;

    // ====================================================================================================
    //                     Start Functions
    // ====================================================================================================
    #region Start
    public void Start(){
        // Assertion Check
        Debug.Assert(content, "content is missing");
        Debug.Assert(contentBlock, "contentBlock is missing - assign the UIBlock component");
        Debug.Assert(questionText, "questionText is missing");
        Debug.Assert(answerChoiceList.Count > 0, "answerChoiceList is empty");
        
        originalPosition = contentBlock.Position.Value;
        bottomPosition = new Vector3(originalPosition.x, originalPosition.y - 1000f, originalPosition.z);
        
        foreach(MathAnswerChoice answerChoice in answerChoiceList){
            answerChoice.OnAnswerChoicePicked += MathAnswerChoice_OnAnswerChoicePickedEventArgs;
        }
        
        contentBlock.Position.Value = bottomPosition;
        content.SetActive(false);
        if(startMinigameOnRun) StartMinigame();
    }

    void OnDestroy(){
        if(wrongAnswerDelayCoroutine != null) StopCoroutine(wrongAnswerDelayCoroutine);
        if(animationCoroutine != null) StopCoroutine(animationCoroutine);
    }
    #endregion

    // ====================================================================================================
    //                     Event Functions
    // ====================================================================================================
    #region Event
    void MathAnswerChoice_OnAnswerChoicePickedEventArgs(
        object sender, MathAnswerChoice.OnAnswerChoicePickedEventArgs e
    ) {
        if(!isWaitingForWrongAnswer){
            AnswerQuestion(e.answerNumber, (MathAnswerChoice)sender);
        }
    }
    #endregion

    // ====================================================================================================
    //                     Minigame Functions
    // ====================================================================================================
    #region Minigame
    public void StartMinigame(){
        // PlayerInteraction.Instance.skipMinigame
        if(false){
            EndMinigame();
            return;
        }

        questionAmount = UnityEngine.Random.Range(minQuestionAmount, maxQuestionAmount);
        answeredAmount = 0;
        correctAnswerAmount = 0;
        wrongAnswerAmount = 0;
        isWaitingForWrongAnswer = false;
        
        GenerateQuestion();
        StartCoroutine(AnimateSlideUp());
    }
    
    public void EndMinigame(){
        Debug.Log(
            "Minigame Finished\n" +
            $"Total Questions: {questionAmount}\n" +
            $"Correct: {correctAnswerAmount}\n" +
            $"Wrong: {wrongAnswerAmount}\n" +
            $"Result: {GetCorrectAnswerPercentage()}%"
        );
        
        // Slide down animation before hiding
        StartCoroutine(AnimateSlideDown());
        // PlayerInteraction.Instance.hasMoney = true;
        vendingMachine.GetComponent<InteractableObject>().enabled = true;
        richKid.SelfDestruct();

        // Reset any pending wrong answer states
        if(wrongAnswerDelayCoroutine != null){
            StopCoroutine(wrongAnswerDelayCoroutine);
            wrongAnswerDelayCoroutine = null;
        }
        isWaitingForWrongAnswer = false;
        
        SetAnswerChoicesInteractable(true);
    }
    
    IEnumerator AnimateSlideUp(){
        if(animationCoroutine != null) StopCoroutine(animationCoroutine);
        
        content.SetActive(true);
        
        float elapsed = 0f;
        Vector3 startPos = bottomPosition;
        Vector3 targetPos = originalPosition;
        
        while(elapsed < slideDuration){
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            contentBlock.Position.Value = Vector3.Lerp(startPos, targetPos, smoothT);
            yield return null;
        }
        
        contentBlock.Position.Value = originalPosition;
        
        elapsed = 0f;
        Vector3 bounceTarget = originalPosition + new Vector3(0, bounceAmount, 0);
        Vector3 returnTarget = originalPosition;
        
        while(elapsed < bounceDuration * 0.5f){
            elapsed += Time.deltaTime;
            float t = elapsed / (bounceDuration * 0.5f);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            contentBlock.Position.Value = Vector3.Lerp(originalPosition, bounceTarget, smoothT);
            yield return null;
        }
        
        elapsed = 0f;
        while(elapsed < bounceDuration * 0.5f){
            elapsed += Time.deltaTime;
            float t = elapsed / (bounceDuration * 0.5f);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            contentBlock.Position.Value = Vector3.Lerp(bounceTarget, returnTarget, smoothT);
            yield return null;
        }
        
        contentBlock.Position.Value = originalPosition;
        animationCoroutine = null;
    }
    
    IEnumerator AnimateSlideDown(){
        if(animationCoroutine != null) StopCoroutine(animationCoroutine);
        
        float elapsed = 0f;
        Vector3 startPos = originalPosition;
        Vector3 targetPos = bottomPosition;
        
        while(elapsed < slideDuration){
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            contentBlock.Position.Value = Vector3.Lerp(startPos, targetPos, smoothT);
            yield return null;
        }
        
        contentBlock.Position.Value = bottomPosition;
        content.SetActive(false);
        animationCoroutine = null;
    }
    #endregion

    // ====================================================================================================
    //                     Math Functions
    // ====================================================================================================
    #region Math
    public void GenerateQuestion(){
        // Reset wrong answer state for new question
        isWaitingForWrongAnswer = false;
        
        // Re-enable all answer choices
        SetAnswerChoicesInteractable(true);
        
        // Create number
        int leftNumber = UnityEngine.Random.Range(minQuestionVariance, maxQuestionVariance);
        int rightNumber = UnityEngine.Random.Range(minQuestionVariance, maxQuestionVariance);
        // Apply operator
        int operatorChoiceIndex = UnityEngine.Random.Range(0, 4);
        string operatorText = "";
        switch (operatorChoiceIndex){
            case 0:{
                    correctAnswerNumber = leftNumber + rightNumber;
                    operatorText = "+";
                    break;
                }
            case 1:{
                    correctAnswerNumber = leftNumber - rightNumber;
                    operatorText = "-";
                    break;
                }
            case 2:{
                    correctAnswerNumber = leftNumber * rightNumber;
                    operatorText = "x";
                    break;
                }
            case 3:{
                    // Division: ensure we get whole numbers
                    if(rightNumber == 0) rightNumber = 1;
                    correctAnswerNumber = leftNumber / rightNumber;
                    leftNumber = correctAnswerNumber * rightNumber;
                    operatorText = "÷";
                    break;
                }
        }
        // Set question text
        questionText.Text = $"{leftNumber} {operatorText} {rightNumber}?";
        questionCount.Text = $"Question {answeredAmount + 1}/{questionAmount}";
        // Create answer choices
        List<int> answerChoiceNumberList = new List<int>();
        int answerOffset = 0;
        answerChoiceNumberList.Add(correctAnswerNumber);
        while (answerChoiceNumberList.Count < answerChoiceList.Count){
            answerOffset += UnityEngine.Random.Range(minQuestionVariance, maxQuestionVariance);
            int newWrongAnswerNumber = correctAnswerNumber + answerOffset;
            if(newWrongAnswerNumber != correctAnswerNumber){
                answerChoiceNumberList.Add(newWrongAnswerNumber);
            }
        }
        ShuffleList(answerChoiceNumberList);
        // Set all answer choices
        int index = 0;
        foreach(MathAnswerChoice answerChoice in answerChoiceList){
            answerChoice.SetAnswerChoice(answerChoiceNumberList[index]);
            index++;
        }
    }

    void AnswerQuestion(int answerNumber, MathAnswerChoice selectedChoice){
        bool isCorrect = answerNumber == correctAnswerNumber;
        bool isDuplicate = questionText.Text == fuckeryFuckText;
        if(isDuplicate) return;

        if(isCorrect){
            fuckeryFuckText = questionText.Text;
            correctAnswerAmount++;
            answeredAmount++;
            
            selectedChoice.AnimateAnswerFeedback(true);

            if(answeredAmount >= questionAmount) StartCoroutine(DelayedEndMinigame());
            else StartCoroutine(DelayedNextQuestion());
            
            AudioManager.Instance.PlaySFX(correctAnswerSfxClip);
        }
        else{
            wrongAnswerAmount++;
            
            selectedChoice.AnimateAnswerFeedback(false);
            SetAnswerChoicesInteractable(false);
            
            isWaitingForWrongAnswer = true;
            if(wrongAnswerDelayCoroutine != null) StopCoroutine(wrongAnswerDelayCoroutine);
            wrongAnswerDelayCoroutine = StartCoroutine(WrongAnswerDelayCoroutine(selectedChoice));
        }
    }

    IEnumerator WrongAnswerDelayCoroutine(MathAnswerChoice wrongChoice){
        yield return new WaitForSeconds(wrongAnswerDelay);
        
        isWaitingForWrongAnswer = false;
        wrongAnswerDelayCoroutine = null;
        
        SetAnswerChoicesInteractable(true);
        wrongChoice.ResetAnswerColor();
    }

    IEnumerator DelayedNextQuestion(){
        yield return new WaitForSeconds(0.5f);
        GenerateQuestion();
    }

    IEnumerator DelayedEndMinigame(){
        yield return new WaitForSeconds(0.5f);
        EndMinigame();
    }

    void SetAnswerChoicesInteractable(bool interactable){
        foreach(MathAnswerChoice answerChoice in answerChoiceList){
            answerChoice.SetInteractable(interactable);
        }
    }

    public float GetCorrectAnswerPercentage(){
        float result = (float)correctAnswerAmount / (float)questionAmount;
        return result * 100.0f;
    }
    #endregion
    
    // ====================================================================================================
    //                     Helper Functions
    // ====================================================================================================
    #region Helper
    void ShuffleList(List<int> list){
        for (int i = 0; i < list.Count; i++){
            int rand = UnityEngine.Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
    #endregion
}
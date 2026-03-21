using System.Collections.Generic;
using Nova;
using UnityEngine;
using UnityEngine.Events;

public class MathMinigameController : MonoBehaviour
{
    public UnityEvent OnMinigameFinished;

    [Header("Component and Object")]
    [SerializeField] private GameObject content;
    [SerializeField] private TextBlock questionText;
    [SerializeField] private List<MathAnswerChoice> answerChoiceList;
    [Header("Minigame Settings")]
    [SerializeField] private int minQuestionAmount = 3;
    [SerializeField] private int maxQuestionAmount = 3;
    [Tooltip("The minimum number a question can create")]
    [SerializeField] private int minQuestionVariance = 0;
    [Tooltip("The maximum number a question can create")]
    [SerializeField] private int maxQuestionVariance = 9;
    [Header("Debug")]
    [Tooltip("Start the minigame when played, default is false")]
    [SerializeField] private bool startMinigameOnRun = false;

    // Gameflow
    private int questionAmount = 0;
    private int answeredAmount = 0;
    private int correctAnswerAmount = 0;
    private int wrongAnswerAmount = 0;
    // Math
    private int correctAnswerNumber = 0;

    // ====================================================================================================
    //                     Start Functions
    // ====================================================================================================
    #region Start
    public void Start()
    {
        // Assertion Check
        Debug.Assert(content, "content is missing");
        Debug.Assert(questionText, "questionText is missing");
        Debug.Assert(answerChoiceList.Count > 0, "answerChoiceList is empty");
        // Connect events
        foreach(MathAnswerChoice answerChoice in answerChoiceList)
        {
            answerChoice.OnAnswerChoicePicked += MathAnswerChoice_OnAnswerChoicePickedEventArgs;
        }
        // Initialize
        content.SetActive(false);
        if (startMinigameOnRun) StartMinigame();
    }
    #endregion

    // ====================================================================================================
    //                     Event Functions
    // ====================================================================================================
    #region Event
    private void MathAnswerChoice_OnAnswerChoicePickedEventArgs(
        object sender, MathAnswerChoice.OnAnswerChoicePickedEventArgs e
    ) {AnswerQuestion(e.answerNumber);}
    #endregion

    // ====================================================================================================
    //                     Minigame Functions
    // ====================================================================================================
    #region Minigame
    public void StartMinigame()
    {
        questionAmount = UnityEngine.Random.Range(minQuestionAmount, maxQuestionAmount);
        answeredAmount = 0;
        correctAnswerAmount = 0;
        wrongAnswerAmount = 0;
        content.SetActive(true);
        GenerateQuestion();
    }
    
    public void EndMinigame()
    {
        Debug.Log(
            "Minigame Finished\n" +
            $"Total Questions: {questionAmount}\n" +
            $"Correct: {correctAnswerAmount}\n" +
            $"Wrong: {wrongAnswerAmount}\n" +
            $"Result: {GetCorrectAnswerPercentage()}%"
        );
        content.SetActive(false);
        OnMinigameFinished?.Invoke();
    }
    #endregion

    // ====================================================================================================
    //                     Math Functions
    // ====================================================================================================
    #region Math
    public void GenerateQuestion()
    {
        // Create number
        int leftNumber = UnityEngine.Random.Range(minQuestionVariance, maxQuestionVariance);
        int rightNumber = UnityEngine.Random.Range(minQuestionVariance, maxQuestionVariance);
        // Apply operator
        int operatorChoiceIndex = UnityEngine.Random.Range(0, 3);
        string operatorText = "";
        switch (operatorChoiceIndex)
        {
            case 0:
                {
                    correctAnswerNumber = leftNumber + rightNumber;
                    operatorText = "+";
                    break;
                }
            case 1:
                {
                    correctAnswerNumber = leftNumber - rightNumber;
                    operatorText = "-";
                    break;
                }
            case 2:
                {
                    correctAnswerNumber = leftNumber * rightNumber;
                    operatorText = "x";
                    break;
                }
            case 3:
                {
                    correctAnswerNumber = leftNumber;
                    leftNumber = correctAnswerNumber * rightNumber;
                    operatorText = "÷";
                    break;
                }
        }
        // Set question text
        questionText.Text = $"{leftNumber} {operatorText} {rightNumber}?";
        // Create answer choices
        List<int> answerChoiceNumberList = new List<int>();
        int answerOffset = 0;
        answerChoiceNumberList.Add(correctAnswerNumber);
        while (answerChoiceNumberList.Count < answerChoiceList.Count)
        {
            answerOffset += UnityEngine.Random.Range(minQuestionVariance, maxQuestionVariance);
            int newWrongAnswerNumber = correctAnswerNumber + answerOffset;
            if (newWrongAnswerNumber != correctAnswerNumber)
            {
                answerChoiceNumberList.Add(newWrongAnswerNumber);
            }
        }
        ShuffleList(answerChoiceNumberList);
        // Set all answer choices
        int index = 0;
        foreach(MathAnswerChoice answerChoice in answerChoiceList)
        {
            answerChoice.SetAnswerChoice(answerChoiceNumberList[index]);
            index++;
        }
    }

    private void AnswerQuestion(int answerNumber)
    {
        // Check the answer
        if (answerNumber == correctAnswerNumber) correctAnswerAmount++;
        else wrongAnswerAmount++;
        answeredAmount++;
        // Check is question available
        if (answeredAmount >= questionAmount) EndMinigame();
        else GenerateQuestion();
    }

    public float GetCorrectAnswerPercentage()
    {
        float result = (float)correctAnswerAmount / (float)questionAmount;
        return result * 100.0f;
    }
    #endregion
    // ====================================================================================================
    //                     Helper Functions
    // ====================================================================================================
    #region Helper
    private void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
    #endregion
}

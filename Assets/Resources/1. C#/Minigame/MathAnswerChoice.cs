using System;
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

    private int currentAnswerNumber = 0;

    // ====================================================================================================
    //                     Start Functions
    // ====================================================================================================
    #region Start
    public void Start()
    {
        // Assertion Check
        Debug.Assert(answerText, "answerText is missing");
    }
    #endregion

    // ====================================================================================================
    //                     Answer Choice Functions
    // ====================================================================================================
    #region Answer Choice
    public void SetAnswerChoice(int number)
    {
        currentAnswerNumber = number;
        answerText.Text = number.ToString();
    }

    public void AnswerChoicePicked()
    {
        OnAnswerChoicePicked?.Invoke(
            this,
            new OnAnswerChoicePickedEventArgs{answerNumber = currentAnswerNumber}
        );
    }
    #endregion
}

using Nova;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class DefaultButton : MonoBehaviour
{
    [Header("Audio Data")]
    [SerializeField] protected string hoverSFXName = "";
    [SerializeField] protected string pressedSFXName = "";

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    private void Start()
    {
        UIBlock uiBlock = GetComponent<Interactable>().UIBlock;
        uiBlock.AddGestureHandler<Gesture.OnUnhover>(ButtonNormal);
        uiBlock.AddGestureHandler<Gesture.OnHover>(ButtonHover);
        uiBlock.AddGestureHandler<Gesture.OnPress>(ButtonPressed);
    }

    private void OnEnable(){ResetButton();}
    #endregion
    // ====================================================================================================
    //                     Interactable Functions
    // ====================================================================================================
    #region Button
    virtual public void ResetButton(){}

    virtual public void ButtonNormal(Gesture.OnUnhover evt) {}

    virtual public void ButtonHover(Gesture.OnHover evt)
    {
        // Play SFX
        if (hoverSFXName != ""){AudioManager.Instance.PlaySFX(hoverSFXName);}
    }

    virtual public void ButtonPressed(Gesture.OnPress evt)
    {
        // Play SFX
        if (pressedSFXName != ""){AudioManager.Instance.PlaySFX(pressedSFXName);}
    }
    #endregion
}

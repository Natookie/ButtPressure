using Nova;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class DefaultButton : MonoBehaviour
{
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
    #endregion
    // ====================================================================================================
    //                     Interactable Functions
    // ====================================================================================================
    #region Button
    virtual public void ButtonNormal(Gesture.OnUnhover evt) {}

    virtual public void ButtonHover(Gesture.OnHover evt) {}

    virtual public void ButtonPressed(Gesture.OnPress evt) {}
    #endregion
}

using Nova;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

[RequireComponent(typeof(Interactable))]
public class BasicEffectButton : DefaultButton
{
    [Header("Scale Effect")]
    [SerializeField] private float normalScale = 1.0f;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressedScale = 0.9f;
    [SerializeField] private float scaleDuration = 0.2f;
    [Header("Text Color Effect")]
    [SerializeField] private TextBlock textBlock;
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color hoverTextColor = Color.gray8;
    [SerializeField] private Color pressedTextColor = Color.gray5;
    [SerializeField] private float textColorDuration = 0.2f;

    private Tween scaleTween;
    private Tween textColorTween;

    // ====================================================================================================
    //                     Interactable Functions
    // ====================================================================================================
    #region Button
    override public void ResetButton()
    {
        transform.localScale = Vector3.one * normalScale;
        textBlock.Color = normalTextColor;
    }

    override public void ButtonNormal(Gesture.OnUnhover evt)
    {
        // Do scale effect
        if (!scaleTween.IsUnityNull()) scaleTween.Kill();
        scaleTween = DOTween.To(
            ()=>transform.localScale,
            x=>transform.localScale = x,
            Vector3.one * normalScale,
            scaleDuration
        );
        // Do text color effect
        if (!textColorTween.IsUnityNull()) textColorTween.Kill();
        textColorTween = DOTween.To(
            ()=>textBlock.Color,
            x=>textBlock.Color = x,
            normalTextColor,
            textColorDuration
        );
    }

    override public void ButtonHover(Gesture.OnHover evt)
    {
        // Play SFX
        if (hoverSFXName != ""){AudioManager.Instance.PlaySFX(hoverSFXName);}
        // Do scale effect
        if (!scaleTween.IsUnityNull()) scaleTween.Kill();
        scaleTween = DOTween.To(
            ()=>transform.localScale,
            x=>transform.localScale = x,
            Vector3.one * hoverScale,
            scaleDuration
        );
        // Do text color effect
        if (!textColorTween.IsUnityNull()) textColorTween.Kill();
        textColorTween = DOTween.To(
            ()=>textBlock.Color,
            x=>textBlock.Color = x,
            hoverTextColor,
            textColorDuration
        );
    }

    override public void ButtonPressed(Gesture.OnPress evt)
    {
        // Play SFX
        if (pressedSFXName != ""){AudioManager.Instance.PlaySFX(pressedSFXName);}
        // Do scale effect
        if (!scaleTween.IsUnityNull()) scaleTween.Kill();
        scaleTween = DOTween.To(
            ()=>transform.localScale,
            x=>transform.localScale = x,
            Vector3.one * pressedScale,
            scaleDuration
        );
        // Do text color effect
        if (!textColorTween.IsUnityNull()) textColorTween.Kill();
        textColorTween = DOTween.To(
            ()=>textBlock.Color,
            x=>textBlock.Color = x,
            pressedTextColor,
            textColorDuration
        );
    }
    #endregion
}

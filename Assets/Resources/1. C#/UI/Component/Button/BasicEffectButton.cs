using Nova;
using UnityEngine;
using DG.Tweening;

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

    void OnDestroy()
    {
        scaleTween?.Kill();
        textColorTween?.Kill();
    }

    private void AnimateTo(float targetScale, Color targetColor)
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(Vector3.one * targetScale, scaleDuration);

        if (textBlock != null)
        {
            textColorTween?.Kill();
            textColorTween = DOTween.To(
                () => textBlock.Color,
                x => textBlock.Color = x,
                targetColor,
                textColorDuration
            );
        }
    }

    override public void ResetButton()
    {
        scaleTween?.Kill();
        textColorTween?.Kill();
        transform.localScale = Vector3.one * normalScale;
        if (textBlock != null) textBlock.Color = normalTextColor;
    }

    override public void ButtonNormal(Gesture.OnUnhover evt)
        => AnimateTo(normalScale, normalTextColor);

    override public void ButtonHover(Gesture.OnHover evt)
    {
        if (hoverSFXName != "") AudioManager.Instance.PlaySFX(hoverSFXName);
        AnimateTo(hoverScale, hoverTextColor);
    }

    override public void ButtonPressed(Gesture.OnPress evt)
    {
        if (pressedSFXName != "") AudioManager.Instance.PlaySFX(pressedSFXName);
        AnimateTo(pressedScale, pressedTextColor);
    }
}
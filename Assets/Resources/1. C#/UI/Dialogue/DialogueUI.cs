using UnityEngine;
using Nova;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [Header("UI REFERENCES")]
    [SerializeField] private UIBlock2D gradientBlock;
    [SerializeField] private float hoverDuration = 0.5f;
    
    private float upPosition = 0f;
    private float downPosition = 0f;
    private Coroutine currentHoverRoutine;

    void Start(){
        downPosition = -gradientBlock.Size.Y.Value;
        upPosition = 0f;
        
        gradientBlock.Position.Y.Value = downPosition;
        PopGradient(false);
    }

    public void PopGradient(bool show, System.Action onComplete = null){
        if(currentHoverRoutine != null) StopCoroutine(currentHoverRoutine);
        
        float targetPosition = show ? upPosition : downPosition;
        currentHoverRoutine = StartCoroutine(HoverGradient(targetPosition, onComplete));
    }

    IEnumerator HoverGradient(float targetPosition, System.Action onComplete){
        float startPosition = gradientBlock.Position.Y.Value;
        float elapsed = 0f;

        while(elapsed < hoverDuration){
            elapsed += Time.deltaTime;
            float t = elapsed / hoverDuration;
            
            gradientBlock.Position.Y.Value = Mathf.Lerp(startPosition, targetPosition, t);
            
            yield return null;
        }

        gradientBlock.Position.Y.Value = targetPosition;
        onComplete?.Invoke();
    }

    public float GradientPos() => gradientBlock.Position.Y.Value;
}
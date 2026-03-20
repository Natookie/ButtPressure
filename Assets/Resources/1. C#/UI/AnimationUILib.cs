using UnityEngine;
using Nova;
using System.Collections;

public class AnimationLib : MonoBehaviour
{
    public static AnimationLib Instance { get; private set; }

    [Header("UI REFERENCES")]
    [SerializeField] private UIBlock2D pooMeter;
    [SerializeField] private UIBlock2D locationUI;
    [SerializeField] private UIBlock2D objectiveUI;

    [Header("ANIMATION SETTINGS")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float popScale = 1.2f;
    [SerializeField] private float topOffset = 200f;

    private float pooOriginalY;
    private float locationOriginalY;
    private Vector3 objectiveOriginalScale;
    private float pooOriginalX;
    private float locationOriginalX;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        if(pooMeter != null){
            pooOriginalX = pooMeter.Position.X.Value;
            pooOriginalY = pooMeter.Position.Y.Value;
            pooMeter.Position.Y.Value = pooOriginalY + topOffset;
        }

        if(locationUI != null){
            locationOriginalX = locationUI.Position.X.Value;
            locationOriginalY = locationUI.Position.Y.Value;
            locationUI.Position.Y.Value = locationOriginalY + topOffset;
        }

        if(objectiveUI != null){
            objectiveOriginalScale = objectiveUI.transform.localScale;
            objectiveUI.transform.localScale = Vector3.zero;
        }
    }

    public void SlideInPooMeter(){
        StartCoroutine(SlideY(pooMeter, pooOriginalY, slideDuration));
    }

    public void SlideInLocationUI(){
        StartCoroutine(SlideY(locationUI, locationOriginalY, slideDuration));
    }

    public void PopInObjectiveUI(){
        StartCoroutine(PopIn(objectiveUI, objectiveOriginalScale, popDuration));
    }

    IEnumerator SlideY(UIBlock2D uiElement, float targetY, float duration){
        float elapsed = 0f;
        float startY = uiElement.Position.Y.Value;

        while(elapsed < duration){
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1, t);
            
            uiElement.Position.Y.Value = Mathf.Lerp(startY, targetY, t);
            yield return null;
        }

        uiElement.Position.Y.Value = targetY;
    }

    IEnumerator PopIn(UIBlock2D uiElement, Vector3 targetScale, float duration){
        uiElement.transform.localScale = Vector3.zero;
        
        float elapsed = 0f;
        while(elapsed < duration * 0.5f){
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            uiElement.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * popScale, t);
            yield return null;
        }

        elapsed = 0f;
        while(elapsed < duration * 0.5f){
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            uiElement.transform.localScale = Vector3.Lerp(Vector3.one * popScale, Vector3.one, t);
            yield return null;
        }

        uiElement.transform.localScale = Vector3.one;
    }

    public void SlideOutPooMeter(){
        StartCoroutine(SlideY(pooMeter, pooOriginalY + topOffset, slideDuration));
    }

    public void SlideOutLocationUI(){
        StartCoroutine(SlideY(locationUI, locationOriginalY + topOffset, slideDuration));
    }

    public void PopOutObjectiveUI(){
        StartCoroutine(PopOut(objectiveUI, objectiveOriginalScale, popDuration));
    }

    IEnumerator PopOut(UIBlock2D uiElement, Vector3 targetScale, float duration){
        float elapsed = 0f;
        while(elapsed < duration * 0.5f){
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            uiElement.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * popScale, t);
            yield return null;
        }

        elapsed = 0f;
        while(elapsed < duration * 0.5f){
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            uiElement.transform.localScale = Vector3.Lerp(Vector3.one * popScale, Vector3.zero, t);
            yield return null;
        }

        uiElement.transform.localScale = Vector3.zero;
    }
}
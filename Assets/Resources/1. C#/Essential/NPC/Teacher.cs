using UnityEngine;
using System.Collections;

public class Teacher : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Sprite[] anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Header("ANIMATION SETTINGS")]
    [SerializeField] private float frameRate = 10f;
    
    private int currentFrame = 0;
    private Coroutine animationCoroutine;
    private bool isAnimating = false;

    void Start(){
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if(anim != null && anim.Length > 0 && spriteRenderer != null)
            spriteRenderer.sprite = anim[0];
    }

    void Update(){
        bool isTyping = DialogueManager.Instance != null && DialogueManager.Instance.IsTypingActive();
        
        if(isTyping && !isAnimating) StartAnimation();
        else if(!isTyping && isAnimating) StopAnimation();
    }
    
    void StartAnimation(){
        if(anim == null || anim.Length <= 1) return;
        
        isAnimating = true;
        currentFrame = 0;
        
        if(animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateSprites());
    }
    
    void StopAnimation(){
        isAnimating = false;
        
        if(animationCoroutine != null){
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        
        if(anim != null && anim.Length > 0 && spriteRenderer != null)
            spriteRenderer.sprite = anim[0];
    }
    
    IEnumerator AnimateSprites(){
        while(isAnimating){
            currentFrame = (currentFrame + 1) % anim.Length;
            
            if(spriteRenderer != null && anim[currentFrame] != null)
                spriteRenderer.sprite = anim[currentFrame];
            
            yield return new WaitForSeconds(1f / frameRate);
        }
    }
    
    public void SetAnimationSpeed(float speed){
        frameRate = Mathf.Max(1f, speed);
        if(isAnimating) StartAnimation();
    }
    
    public void SetSprites(Sprite[] newSprites){
        anim = newSprites;
        
        if(anim != null && anim.Length > 0 && spriteRenderer != null)
            spriteRenderer.sprite = anim[0];
            
        if(isAnimating) StartAnimation();
    }
}
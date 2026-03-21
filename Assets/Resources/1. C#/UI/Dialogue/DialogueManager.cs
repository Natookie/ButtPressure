using UnityEngine;
using Nova;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("DIALOGUE CONFIG")]
    [SerializeField] private float typingSpeed = 30f;
    [SerializeField] private float punctuationDelay = 0.3f;
    [SerializeField] private float tildePauseDuration = 1f;
    [Space(10)]
    [SerializeField] private float fastForwardSpeed = 100f;
    [SerializeField] private float fastForwardPunctuationDelay = 0.05f;
    [SerializeField] private float fastForwardTildePauseDuration = 0.05f;

    [Header("EXTRAS")]
    [SerializeField] private UIBlock2D skipButton;
    [SerializeField] private Color32 skipHoverColor;
    [SerializeField] private Color32 skipUnhoverColor;

    [Header("AUDIO SETTINGS")]
    [SerializeField] private string skipPressedSFXKey = "buttonPress";

    [Header("REFERENCES")]
    [SerializeField] private DialogueUI dialogueUI;

    public ItemView dialogueItemVisual;
    private DialogueItemVisual visual;
    private GameObject visualGameObject;

    private Coroutine typingRoutine;
    private bool isTyping = false;
    public bool IsTyping => isTyping;
    private bool skipAllDialogues = false;
    private bool isFastForwarding = false;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        visual = dialogueItemVisual.Visuals as DialogueItemVisual;
        if(visual == null) Debug.LogError("dialogueItemVisual.Visuals is not a DialogueItemVisual!");
        
        if(visual != null) visualGameObject = dialogueItemVisual.gameObject;
        
        skipButton.AddGestureHandler<Gesture.OnPress>(skipClick);
        skipButton.AddGestureHandler<Gesture.OnHover>(skipHover);
        skipButton.AddGestureHandler<Gesture.OnUnhover>(skipUnhover);
        
        if(visualGameObject != null){
            ShowDialogueUI();
            HideDialogueUI();
        }
    }

    void Update(){
        if(!GameManager.Instance.isInitialized) return;
        if(skipButton != null) skipButton.transform.position = new Vector3(-1000, -1000, -1000);
    }

    public void SetDialogue(DLib.Character character, string content){
        SetDialogue(character.Name, character.Color, content);
    }

    void SetDialogue(string characterName, Color characterColor, string content){
        if(visual == null) return;
        
        if(typingRoutine != null) StopCoroutine(typingRoutine);

        visual.dialogueName.Text = characterName;
        visual.dialogueName.Color = characterColor;
        visual.dialogueSeparator.Color = characterColor;
        visual.dialogueContent.Text = "";

        if(skipAllDialogues) visual.dialogueContent.Text = StripTags(content);
        else typingRoutine = StartCoroutine(TypeText(content));
    }

    string StripTags(string text){
        string result = text.Replace("~", "").Replace("|", "");
        return result;
    }

    IEnumerator TypeText(string text){
        if(skipAllDialogues){
            visual.dialogueContent.Text = StripTags(text);
            yield break;
        }
        
        isTyping = true;

        int i = 0;
        string displayedText = "";
        while(i < text.Length && !skipAllDialogues){
            float currentTypingSpeed = (isFastForwarding) ? fastForwardSpeed : typingSpeed;
            float currentPunctuationDelay = (isFastForwarding) ? fastForwardPunctuationDelay : punctuationDelay;
            float currentTildePause = (isFastForwarding) ? fastForwardTildePauseDuration : tildePauseDuration;
            
            if(text[i] == '<'){
                int tagEnd = text.IndexOf('>', i);
                if(tagEnd != -1){
                    string tagContent = text.Substring(i, tagEnd - i + 1);
                    displayedText += tagContent;
                    visual.dialogueContent.Text = displayedText;
                    i = tagEnd + 1;
                    continue;
                }
            }else if(text[i] == '~'){
                yield return new WaitForSeconds(currentTildePause);
                i++;
                continue;
            }else if(text[i] == '|'){
                yield return new WaitForSeconds((isFastForwarding) ? 0f : .1f);
                isTyping = false;
                i++;
                continue;
            }
            
            displayedText += text[i];
            visual.dialogueContent.Text = displayedText;
            if(text[i] == ',' || text[i] == '.') yield return new WaitForSeconds(currentPunctuationDelay);
            else yield return new WaitForSeconds(1f / currentTypingSpeed);
            i++;
        }
        
        if(skipAllDialogues){
            visual.dialogueContent.Text = StripTags(text);
            isTyping = false;
        }else{
            yield return new WaitForSeconds((isFastForwarding) ? 0.1f : 1f);
            isTyping = false;
        }
    }

    public void SkipTyping(){
        skipAllDialogues = true;
        if(typingRoutine != null) StopCoroutine(typingRoutine);
        isTyping = false;
    }

    public void SetFastForward(bool fastForward){
        isFastForwarding = fastForward;
        
        if(visual != null){
            if(fastForward) visual.dialogueContent.Color = new Color(0.8f, 0.8f, 0.8f);
            else visual.dialogueContent.Color = Color.white;
        }
    }

    public void ForceStopDialogue(){
        if(typingRoutine != null){
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }
        
        isTyping = false;
        HideDialogueUI();
    }

    public void ResetSkip() => skipAllDialogues = false;
    public bool IsTypingActive() => isTyping && !skipAllDialogues;

    public void ShowDialogueUI(System.Action onComplete = null){
        PlayerMovement.Instance.canMove = false;

        if(dialogueUI != null) dialogueUI.PopGradient(true, () => {
            if(visualGameObject != null) visualGameObject.SetActive(true);
            onComplete?.Invoke();
        });
    }

    public void HideDialogueUI(){
        PlayerMovement.Instance.canMove = true;

        if(visualGameObject != null) visualGameObject.SetActive(false);
        if(dialogueUI != null) dialogueUI.PopGradient(false);
    }

    #region SKIP BUTTON
    void skipClick(Gesture.OnPress evt){
        SkipTyping();
        GameManager.Instance.isInitialized = true;
    }
    void skipHover(Gesture.OnHover evt) => StartCoroutine(LerpSkipButton(skipHoverColor, 1.1f));
    void skipUnhover(Gesture.OnUnhover evt) => StartCoroutine(LerpSkipButton(skipUnhoverColor, 1f));

    IEnumerator LerpSkipButton(Color32 targetColor, float targetScale){
        if(skipButton == null) yield break;

        float duration = 0.2f;
        float elapsed = 0f;
        
        Color32 startColor = skipButton.Color;
        Vector3 startScale = skipButton.transform.localScale;
        Vector3 endScale = new Vector3(targetScale, targetScale, targetScale);
        
        while(elapsed < duration && skipButton != null){
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            skipButton.Color = Color32.Lerp(startColor, targetColor, t);
            skipButton.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            
            yield return null;
        }
        
        skipButton.Color = targetColor;
        skipButton.transform.localScale = endScale;
    }
    #endregion
}
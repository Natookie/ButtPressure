using UnityEngine;
using UnityEngine.Events;

public class EventFlag : MonoBehaviour
{
    // Item
    public UnityEvent TotemObtained;
    // Quest
    public UnityEvent BullyQuestStarted;
    public UnityEvent JanitorQuestStarted;
    // Minigame
    public UnityEvent MathMinigameFinished;
    // Misc
    public UnityEvent JanitorMoved;

    public static EventFlag Instance {get; private set;}

    [Header("Item")]
    public bool HasTotem {
        set {hasTotem = value; if (value) TotemObtained?.Invoke();}
        get {return hasTotem;}
    }
    private bool hasTotem;
    public bool hasDrink = false;
    public bool hasMoney = false;

    [Header("Quest")]
    public bool isBullyQuestStarted {set {if (value) BullyQuestStarted?.Invoke();}}
    public bool isJanitorQuestStarted {set {if (value) JanitorQuestStarted?.Invoke();}}

    [Header("Minigame")]
    public bool isMathMinigameFinished {set {if (value) MathMinigameFinished?.Invoke();}}

    [Header("Misc")]
    public bool isJanitorMoved {set {if (value) JanitorMoved?.Invoke();}}

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy() => Instance = null;
}

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
    public UnityEvent HanakoMoved;
    public UnityEvent DoneHailMary;

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
    public bool hasJanitorMoved {set {if (value) JanitorMoved?.Invoke();}}
    public bool HasHanakoMoved {
        set {hasHanakoMoved = value; if (value) HanakoMoved?.Invoke();}
        get {return hasHanakoMoved;}
    }
    private bool hasHanakoMoved;
    public bool HasDoneHailMary{
        set {hasDoneHailMary = value; if (value) DoneHailMary?.Invoke();}
        get {return hasDoneHailMary;}
    }
    private bool hasDoneHailMary;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy() => Instance = null;
}

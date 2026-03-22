using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [Header("CUTSCENE")]
    [SerializeField] private PlayerMovement pm;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform playerStart;
    [Space(10)]
    [SerializeField] private Door classDoor;
    [SerializeField] private GameObject pooMeter;
    [SerializeField] private GameObject locationUI;
    [SerializeField] private GameObject objectiveUI;
    [Space(10)]
    [SerializeField] private bool cutscene;

    [Header("SETTINGS")]
    public bool isInitialized;
    public bool isEnded;
    public bool isInMiniGame;

    private Keyboard keyboard;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void Start(){
        keyboard = Keyboard.current;
        
        if(cutscene){
            pm.SetPosition(playerStart.position);
            StartCoroutine(StartGame());
        }
        else isInitialized = true;
    }

    void Update(){
        if(keyboard != null && keyboard.spaceKey.isPressed) DialogueManager.Instance.SetFastForward(true);
        else DialogueManager.Instance.SetFastForward(false);
    }

    IEnumerator StartGame(){
        yield return new WaitForSeconds(0.1f);
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "[Classroom - First Day]"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.SetDialogue(
            DLib.TEACHER,
            "Alright class, settle down, settle down."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.TEACHER,
            "We have a new student joining us today."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        pm.ForceMove(playerTarget);
        yield return new WaitUntil(() => pm.hasReachedTarget);

        DialogueManager.Instance.SetDialogue(
            DLib.TEACHER,
            "Please introduce yourself."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.STUDENT,
            "Hello everyone, watashiwa namaewa. Udin Desu. \nYoshi onegai si mas."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I like ramen, and i like Honda."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.CROWD,
            "*What in the.. FUCKK is he saying? \n~*I bet he smells like ramen too."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.TEACHER,
            "Umm.. okay then.. Yeaaa. \n~L-l, let's just move on shall we?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.TEACHER,
            "You can take a seat next to Kana there."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "[Classroom - Recess Time]"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "*Looks at Kana*~ \nIta daki mas Kana, nice to meet you <color=#FFA6A6>0///0</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.KANA,
            "How about you go fu-|"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I have a question. Kana..~ \nMay i know where the <color=#E76F2E>restroom</color> is?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I need to..~ I need to take a <color=#8C5A3C>poopie</color> pwease? <color=#FFA6A6>UwU</color>"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.KANA,
            "It's down the hallway, go <color=#E76F2E>left</color> after you exit the class. \n~And please don't talk to me ever again."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        if(classDoor.gameObject.activeSelf) classDoor.Interact();

        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "As you can see, this onyo needs to take a dookie."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        objectiveUI.SetActive(true);
        ObjectiveUI.Instance.SetObjective("Go to a toilet");
        AnimationLib.Instance.PopInObjectiveUI();

        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "Press <color=#408A71>A</color> or <color=#408A71>D</color> to move, and <color=#B0E4CC>E</color> to interact"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "Tips: Hold <color=#408A71>SPACE</color> to fast forward dialogue\nMaking the game easier"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I need to find the restroom <color=#4F6F52>ASAP</color>. \nI can feel it coming out."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        AnimationLib.Instance.SlideInPooMeter();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I'm on 3rd floor right now. Let's try to find it."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        AnimationLib.Instance.SlideInLocationUI();
        DialogueManager.Instance.HideDialogueUI();

        DialogueManager.Instance.ResetSkip();
        isInitialized = true;
    }

    public void EndGame(int type){
        switch(type){
            case 1: //Ran out of time
                break;
            case 2: //Caught by Hanako
                pooMeter.GetComponent<PooMeter>().UpdateMultiplier();
                break;
        }
        isEnded = true;
    }
}
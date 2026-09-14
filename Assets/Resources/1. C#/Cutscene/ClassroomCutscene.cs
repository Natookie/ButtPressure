using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClassroomCutscene : MonoBehaviour
{
    public static ClassroomCutscene Instance {get; private set;}

    [Header("REFERENCES")]
    [SerializeField] private Transform playerStartPosition;
    [SerializeField] private Transform playerMovePosition;
    [SerializeField] private Transform playerExitPosition;
    public string playerExitLocationName;

    [Header("AUDIO")]
    [SerializeField] private AudioClip doorSFX;

    private bool isPlayerMoveFinish = false;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy() => Instance = null;

    void Start()
    {
        // Assertion check
        Debug.Assert(playerStartPosition, "playerStartPosition is missing");
        Debug.Assert(playerMovePosition, "playerMovePosition is missing");
        Debug.Assert(playerExitPosition, "playerExitPosition is missing");
        Debug.Assert(playerExitLocationName != "", "playerExitLocationName is missing");
        // Connect events
        Player.Instance.ForcedMoveCompleted.AddListener(()=>{isPlayerMoveFinish = true;});
    }
    #endregion

    // ====================================================================================================
    //                     Cutscene Functions
    // ====================================================================================================
    #region Cutscene
    public void DoCutscene()
    {
        StartCoroutine(StartCutscene());
    }

    public IEnumerator StartCutscene(){
        Player.Instance.transform.position = playerStartPosition.position;
        GameManager.Instance.DisableEssentialUI();
        Player.Instance.EnableInput = false;

        yield return new WaitForSeconds(0.1f);
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true, true);
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

        isPlayerMoveFinish = false;
        Player.Instance.ForceMove(playerMovePosition.position);
        yield return new WaitUntil(() => isPlayerMoveFinish);

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
            "*What in the.. F**KK is he saying? \n~*I bet he smells like ramen too."
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

        LocationController.Instance.ChangeLocation(playerExitLocationName);  
        Player.Instance.transform.position = playerExitPosition.position;
        AudioManager.Instance.PlaySFX(doorSFX);

        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "As you can see, this onyo needs to take a dookie."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        ObjectiveUI.Instance.gameObject.SetActive(true);
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
        GameManager.Instance.EnableEssentialUI();
    }
    #endregion
}

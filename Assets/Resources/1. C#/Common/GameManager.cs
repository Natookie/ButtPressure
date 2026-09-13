using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEditor;

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
    [SerializeField] private EndgameUI endgameUI;
    [Space(10)]
    [SerializeField] private bool cutscene;
    [SerializeField] private GameObject[] linkedList;

    [Header("SETTINGS")]
    public bool isInitialized;
    public bool isEnded;
    public bool isInMiniGame;

    [Header("AUDIO")]
    [SerializeField] private AudioClip mainMusicAudioClip;

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
        
        if(cutscene && linkedList != null && linkedList.Length > 0){
            foreach(GameObject obj in linkedList){
                if(obj != null) obj.SetActive(false);
            }
            if(linkedList[0] != null) linkedList[0].SetActive(true);
        }
        
        if(cutscene){
            // pm.SetPosition(playerStart.position);
            DisableEssentialUI();
            StartCoroutine(StartGame());
        }else{
            isInitialized = true;
            StartCoroutine(EnableEssentialUI());
        }

        AudioManager.Instance.PlayMusic(mainMusicAudioClip);
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
        
        InteractableObject classDoorInteractable = classDoor.GetComponent<InteractableObject>();
        if(classDoorInteractable) classDoorInteractable.enabled = false;

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
        
        // pm.ForceMove(playerTarget);
        // yield return new WaitUntil(() => pm.hasReachedTarget);

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

        classDoorInteractable = classDoor.GetComponent<InteractableObject>();
        if(classDoorInteractable) classDoorInteractable.enabled = true;
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

    public IEnumerator EnableEssentialUI(){
        yield return null;
        if(pooMeter != null) pooMeter.SetActive(true);
        if(locationUI != null) locationUI.SetActive(true);
        if(objectiveUI != null) objectiveUI.SetActive(true);
        if(ObjectiveUI.Instance != null && string.IsNullOrEmpty(ObjectiveUI.Instance.GetCurrentObjective())) ObjectiveUI.Instance.SetObjective("Non cutscene");
        
        if(AnimationLib.Instance != null){
            AnimationLib.Instance.SlideInPooMeter();
            AnimationLib.Instance.SlideInLocationUI();
            AnimationLib.Instance.PopInObjectiveUI();
        }
    }

    public void DisableEssentialUI(){
        if(pooMeter != null) pooMeter.SetActive(false);
        if(locationUI != null) locationUI.SetActive(false);
        if(objectiveUI != null) objectiveUI.SetActive(false);
    }

    public bool IsEssentialUIEnabled(){
        bool pooEnabled = (pooMeter != null && pooMeter.activeInHierarchy);
        bool locationEnabled = (locationUI != null && locationUI.activeInHierarchy);
        bool objectiveEnabled = (objectiveUI != null && objectiveUI.activeInHierarchy);
        return pooEnabled && locationEnabled && objectiveEnabled;
    }

    public void EndGame(int type){
        switch(type){
            case 1:
                endgameUI.OpenGameoverScreen();
                break;
            case 2:
                pooMeter.GetComponent<PooMeter>().UpdateMultiplier();
                endgameUI.OpenGameoverScreen();
                break;
            case 3:
                endgameUI.OpenGamewonScreen();
                break;
        }
        isEnded = true;
    }

    public void ActivateLinkedListObject(int index){
        if(linkedList == null) return;
        if(index < 0 || index >= linkedList.Length) return;
        
        foreach(GameObject obj in linkedList){
            if(obj != null) obj.SetActive(false);
        }
        
        if(linkedList[index] != null) linkedList[index].SetActive(true);
    }

    public void ActivateLinkedListObject(string objectName){
        if(linkedList == null) return;
        
        foreach(GameObject obj in linkedList){
            if(obj != null) obj.SetActive(false);
        }
        
        foreach(GameObject obj in linkedList){
            if(obj != null && obj.name == objectName){
                obj.SetActive(true);
                break;
            }
        }
    }

    public GameObject GetActiveLinkedListObject(){
        if(linkedList == null) return null;
        
        foreach(GameObject obj in linkedList){
            if(obj != null && obj.activeInHierarchy){
                return obj;
            }
        }
        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        
        GameManager manager = (GameManager)target;
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Linked List Controls", EditorStyles.boldLabel);
        
        var linkedListProperty = serializedObject.FindProperty("linkedList");
        int arraySize = linkedListProperty?.arraySize ?? 0;
        
        if(arraySize > 0){
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.green;
            if(GUILayout.Button("Enable Essential UI", GUILayout.Height(30))) manager.EnableEssentialUI();
            GUI.backgroundColor = Color.yellow;
            if(GUILayout.Button("Disable Essential UI", GUILayout.Height(30))) manager.DisableEssentialUI();
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);
            
            for(int i = 0; i < arraySize; i++){
                var element = linkedListProperty.GetArrayElementAtIndex(i);
                if(element.objectReferenceValue != null){
                    string objectName = element.objectReferenceValue.name;
                    bool isActive = ((GameObject)element.objectReferenceValue).activeSelf;
                    
                    GUI.backgroundColor = (isActive) ? Color.green : Color.gray;
                    if(GUILayout.Button($"{i}: {objectName} {(isActive ? "✓" : "")}", GUILayout.Height(30)))
                        manager.ActivateLinkedListObject(i);
                    GUI.backgroundColor = Color.white;
                }
                else{
                    GUI.backgroundColor = Color.red;
                    if(GUILayout.Button($"{i}: MISSING", GUILayout.Height(30))) Debug.LogWarning($"Object at index {i} is missing");
                    GUI.backgroundColor = Color.white;
                }
            }
            
            EditorGUILayout.Space(10);
        }
        else EditorGUILayout.HelpBox("Gay fuck", MessageType.Info);
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
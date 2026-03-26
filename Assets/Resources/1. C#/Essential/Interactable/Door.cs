using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(InteractableObject))]
public class Door : MonoBehaviour, IInteractable
{
    [Header("DOOR SETTINGS")]
    [SerializeField] private Door linkedDoor;
    public bool isLocked = false;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private DoorType doorType = DoorType.Door;
    
    [Header("EVENT SETTINGS")]
    [SerializeField] private bool triggerEvent = false;
    [SerializeField] private UnityEvent onDoorUsed;
    [SerializeField] private UnityEvent onDoorLockedUsed;
    
    [Header("MINIGAME WAIT")]
    [SerializeField] private bool waitForMinigame = false;
    private bool isWaitingForMinigame = false;
    
    private VisualCue visualCue;
    private AudioSource audioSource;

    public enum DoorType{
        Door,
        Stair
    }

    void Start(){
        audioSource = GetComponent<AudioSource>();
        visualCue = VisualCue.Instance;
    }

    public void Interact(){
        if(triggerEvent){
            if(isLocked && triggerEvent) onDoorLockedUsed?.Invoke();
            if(waitForMinigame && triggerEvent){
                isWaitingForMinigame = true;
                onDoorUsed?.Invoke();
            }

            return;
        }

        if(isWaitingForMinigame) return;

        if(linkedDoor != null){
            TeleportToLinkedDoor();
            if(triggerEvent) onDoorUsed?.Invoke();
        }
    }

    public void TeleportToLinkedDoor(){
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player == null) return;
        
        Transform currentParent = transform.parent;
        Transform linkedParent = linkedDoor.transform.parent;
        
        bool hasDifferentParent = currentParent != linkedParent;
        if(hasDifferentParent){
            if(linkedParent != null && !linkedParent.gameObject.activeSelf)
                linkedParent.gameObject.SetActive(true);
            if(currentParent != null && currentParent.gameObject.activeSelf)
                currentParent.gameObject.SetActive(false);
            
            LocationUI.Instance.SetLocation(linkedParent.gameObject.name);
            PlayerCam.Instance.SetConfiner(linkedParent.gameObject.GetComponent<BoxCollider2D>());
        }else{
            if(currentParent != null) LocationUI.Instance.SetLocation(currentParent.gameObject.name);
        }
        
        Vector3 newPosition = linkedDoor.transform.position;
        newPosition.y = player.transform.position.y;
        newPosition.z = player.transform.position.z;
        player.transform.position = newPosition;
        
        InteractableObject linkedInteractable = linkedDoor.GetComponent<InteractableObject>();
        visualCue.SetCurrentInteractable(linkedInteractable);
        
        if(openSound) AudioManager.Instance.PlaySFX(openSound);
    }
    
    public void CompleteMinigame(){
        TeleportToLinkedDoor();
        isWaitingForMinigame = false;
    }
    
    public Door GetLinkedDoor() => linkedDoor;
    public bool IsLocked() => isLocked;
    public void SetLocked(bool locked) => isLocked = locked;
    public DoorType GetDoorType() => doorType;
    
    public void InitializeDoor(bool isStair){
        doorType = (isStair) ? DoorType.Stair : DoorType.Door;
        
        string parentName = transform.parent != null ? transform.parent.name : "Unknown";
        string targetParentName = linkedDoor != null && linkedDoor.transform.parent != null ? linkedDoor.transform.parent.name : "Unknown";
        
        if(isStair) gameObject.name = $"S: {parentName}-{targetParentName}";
        else{
            if(parentName == targetParentName){
                int siblingIndex = transform.GetSiblingIndex();
                gameObject.name = $"D: {parentName}-{targetParentName}_{siblingIndex}";
            }
            else gameObject.name = $"D: {parentName}-{targetParentName}";
            
            InteractableObject interactable = GetComponent<InteractableObject>();
            if(interactable != null) interactable.SetPrompt($"Enter {targetParentName}");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Door))]
public class DoorEditor : Editor
{
    SerializedProperty linkedDoorProp;
    SerializedProperty isLockedProp;
    SerializedProperty openSoundProp;
    SerializedProperty doorTypeProp;
    SerializedProperty triggerEventProp;
    SerializedProperty onDoorUsedProp;
    SerializedProperty onDoorLockedUsedProp;
    SerializedProperty waitForMinigameProp;
    
    bool showMinigameSettings = false;

    void OnEnable(){
        linkedDoorProp = serializedObject.FindProperty("linkedDoor");
        isLockedProp = serializedObject.FindProperty("isLocked");
        openSoundProp = serializedObject.FindProperty("openSound");
        doorTypeProp = serializedObject.FindProperty("doorType");
        triggerEventProp = serializedObject.FindProperty("triggerEvent");
        onDoorUsedProp = serializedObject.FindProperty("onDoorUsed");
        onDoorLockedUsedProp = serializedObject.FindProperty("onDoorLockedUsed");
        waitForMinigameProp = serializedObject.FindProperty("waitForMinigame");
    }
    
    public override void OnInspectorGUI(){
        Door door = (Door)target;
        
        serializedObject.Update();
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(5);
        
        EditorGUILayout.PropertyField(linkedDoorProp);
        EditorGUILayout.PropertyField(isLockedProp);
        EditorGUILayout.PropertyField(openSoundProp);
        EditorGUILayout.PropertyField(doorTypeProp);
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.Space(5);
        
        EditorGUILayout.PropertyField(triggerEventProp);
        
        if(triggerEventProp.boolValue){
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(onDoorUsedProp);
            EditorGUILayout.PropertyField(onDoorLockedUsedProp);
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        showMinigameSettings = EditorGUILayout.Foldout(showMinigameSettings, "Minigame Settings", true);
        
        if(showMinigameSettings){
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(waitForMinigameProp);
            
            if(waitForMinigameProp.boolValue && !triggerEventProp.boolValue)
                EditorGUILayout.HelpBox("Trigger Event must be enabled for minigame", MessageType.Warning);
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Initialize Door", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        
        EditorGUILayout.BeginHorizontal();
        
        if(GUILayout.Button("Door", GUILayout.Height(30))){
            door.InitializeDoor(false);
            EditorUtility.SetDirty(door);
        }
        
        if(GUILayout.Button("Stair", GUILayout.Height(30))){
            door.InitializeDoor(true);
            EditorUtility.SetDirty(door);
        }
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if(GUILayout.Button("Unlock", GUILayout.Height(25))){
            door.SetLocked(false);
            EditorUtility.SetDirty(door);
        }
        
        GUI.backgroundColor = Color.red;
        if(GUILayout.Button("Lock", GUILayout.Height(25))){
            door.SetLocked(true);
            EditorUtility.SetDirty(door);
        }
        
        GUI.backgroundColor = Color.cyan;
        if(GUILayout.Button("Select Linked", GUILayout.Height(25))){
            if(door.GetLinkedDoor() != null){
                Selection.activeGameObject = door.GetLinkedDoor().gameObject;
                EditorGUIUtility.PingObject(door.GetLinkedDoor().gameObject);
            }
        }
        
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        
        GUI.enabled = false;
        EditorGUILayout.TextField("Name", door.name);
        EditorGUILayout.ObjectField("Linked", door.GetLinkedDoor(), typeof(Door), true);
        EditorGUILayout.Toggle("Locked", door.IsLocked());
        EditorGUILayout.EnumPopup("Type", door.GetDoorType());
        GUI.enabled = true;
        
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
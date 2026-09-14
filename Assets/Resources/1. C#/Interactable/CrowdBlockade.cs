using UnityEngine;
using System.Collections;

public class CrowdBlockade : MonoBehaviour, IInteractable
{
    [Header("REFERENCES")]
    [SerializeField] private DoorPush cafeteriaDoorPush;
    public Transform cameraFollow;

    [HideInInspector] public bool hasIntroduced;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Start()
    {
        // Assertion check
        Debug.Assert(cafeteriaDoorPush, "cafeteriaDoorPush is missing");
        Debug.Assert(cameraFollow, "cameraFollow is missing");
        // Connect events
        EventFlag.Instance.HanakoMoved.AddListener(()=>{gameObject.SetActive(false);});
    }
    
    void OnTriggerEnter2D(Collider2D collider){
        if(collider.CompareTag("Player"))
        {
            if(!hasIntroduced) StartCoroutine(IntroduceProblem());
            else StartCoroutine(RemindProblem());
        }
    }
    #endregion

    // ====================================================================================================
    //                     Interact Functions
    // ====================================================================================================
    #region Interact
    public void Interact() => StartCoroutine(RemindProblem());
    #endregion

    // ====================================================================================================
    //                     Dialogue Functions
    // ====================================================================================================
    #region Dialogue
    IEnumerator IntroduceProblem(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.CROWD,
            "(Crowd sounds crowding)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Excuse me, can I please get through?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.CROWD,
            "(Crowd sounds crowding)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "I can't get through, maybe I'll go around."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.CROWD,
            "(Crowd sounds crowding)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.HideDialogueUI();
        ObjectiveUI.Instance.SetObjective("Find a way to pass the crowd");
        hasIntroduced = true;
        cafeteriaDoorPush.ToggleLock(false);
    }

    IEnumerator RemindProblem(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ChangeFollowTarget(cameraFollow.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.CROWD,
            "(Crowd sounds crowding)"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.HideDialogueUI();
    }
    #endregion
}
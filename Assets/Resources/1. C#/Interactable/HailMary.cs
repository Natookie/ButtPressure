using System.Collections;
using UnityEngine;

public class HailMary : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private Door targetDoor1;
    [SerializeField] private Door targetDoor2;
    [SerializeField] private CrowdBlockade crowdBlockade;
    [SerializeField] private Transform crowdViewPosition;
    [SerializeField] private Transform toiletViewPosition;

    private bool hasIntroduced = false;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void Start()
    {
        // Assertion check
        Debug.Assert(targetDoor1, "targetDoor1 is missing");
        Debug.Assert(targetDoor2, "targetDoor2 is missing");
        Debug.Assert(crowdBlockade, "crowdBlockade is missing");
        Debug.Assert(crowdViewPosition, "crowdViewPosition is missing");
        Debug.Assert(toiletViewPosition, "toiletViewPosition is missing");
    }

    void OnEnable()
    {
        if (EventFlag.Instance.HasHanakoMoved && !hasIntroduced)
        {
            targetDoor1.ToggleLock(true);
            targetDoor2.ToggleLock(true);
            crowdBlockade.gameObject.SetActive(false);
            StartCoroutine(LastHailMary());
        }
    }
    #endregion

    // ====================================================================================================
    //                     Dialogue Functions
    // ====================================================================================================
    #region Dialogue
    public IEnumerator LastHailMary(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);

        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "This Onyo nearly sh*t his pants."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.NARRATOR,
            "Literary."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(crowdViewPosition.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "The crowd is gone."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(toiletViewPosition.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "My only toilet..."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Mae nizuzu-ge, Hail Mary, BANZAAIII!!"
        );
        CameraShake.Instance.ShakeCamera(true);
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        CameraController.Instance.ResetFollowTarget();
        ObjectiveUI.Instance.SetObjective("GO TO THE TOILET!");

        DialogueManager.Instance.HideDialogueUI();
        hasIntroduced = true;
        EventFlag.Instance.HasDoneHailMary = true;
    }
    #endregion
}

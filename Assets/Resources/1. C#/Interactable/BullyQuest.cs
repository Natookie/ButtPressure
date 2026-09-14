using UnityEngine;
using System.Collections;

public class BullyQuest : MonoBehaviour
{
    private bool hasIntroduced = false;
    private bool hasDrink => EventFlag.Instance.hasDrink;

    // ====================================================================================================
    //                     Virtual Functions
    // ====================================================================================================
    #region Virtual
    void OnEnable()
    {
        EnterRoom();
    }
    #endregion

    // ====================================================================================================
    //                     Interact Functions
    // ====================================================================================================
    #region Interact
    public void EnterRoom()
    {
        if(!hasDrink){
            if (!hasIntroduced) StartCoroutine(TriggerQuest());
            else StartCoroutine(RemindQuest());
        }
        else StartCoroutine(CompleteQuest());
    }
    #endregion

    // ====================================================================================================
    //                     Dialogue Functions
    // ====================================================================================================
    #region Dialogue
    public IEnumerator TriggerQuest(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JIN,
            "HUHH?"
        );
        CameraShake.Instance.ShakeCamera(false);
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.SetDialogue(
            DLib.JOHN,
            "WHO THE F- LET YOU INN??"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "But Kana said this is the rest room."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JAKI,
            "OHH, you are the new guy isn't it, a freak show.~\nYou want to take a s**t? We won't let you anyway."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Why?? I need to take a <color=#8C5A3C>poopie</color>. ~dips**t."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JIN,
            "F**k you just say to us?~ Whatever, Saki is blocking the\nstaiway for nerd like you on recess time."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.JIN,
            "There's no toilet on this floor, school layout is f**ked.\nThere is one at <color=#E76F2E>2nd floor</color> tho."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.JIN,
            "But remember, saki is blocking the stairway\nUnless... We give you a special permission to pass."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Give me one then."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JOHN,
            "Sure, <color=#E76F2E>give us a drink</color>, and we will give you this <color=#E76F2E>totem</color>\nto pass"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        CameraController.Instance.ResetFollowTarget();
        ObjectiveUI.Instance.SetObjective("Buy a drink");

        DialogueManager.Instance.HideDialogueUI();
        EventFlag.Instance.isBullyQuestStarted = true;
        hasIntroduced = true;
    }

    public IEnumerator CompleteQuest(){
        yield return new WaitForSeconds(0.1f);

        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Here, i bought a red cola."
        );
        
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JIN,
            "Hoho, nice. Even tho you only bought 1 when there is \nclearly 3 of us. We will let it slide. \nDeveloper is lazy"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        DialogueManager.Instance.SetDialogue(
            DLib.JOHN,
            "Here, take this <color=#E76F2E>totem</color>. Scram off."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());

        CameraController.Instance.ResetFollowTarget();
        ObjectiveUI.Instance.SetObjective("Go to the 2nd floor");

        DialogueManager.Instance.HideDialogueUI();
        EventFlag.Instance.HasTotem = true;
    }

    public IEnumerator RemindQuest(){
        yield return new WaitForSeconds(0.1f);
        
        bool uiReady = false;
        DialogueManager.Instance.ShowDialogueUI(() => uiReady = true);
        yield return new WaitUntil(() => uiReady);
        
        DialogueManager.Instance.SetDialogue(
            DLib.JIN,
            "Yo, you got that drink yet?"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Not yet..."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ChangeFollowTarget(this.transform);
        DialogueManager.Instance.SetDialogue(
            DLib.JOHN,
            "Well hurry up then! We're thirsty!"
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.SetDialogue(
            DLib.JAKI,
            "Go buy it from the vending machine.\nDon't come back empty handed."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        CameraController.Instance.ResetFollowTarget();
        DialogueManager.Instance.SetDialogue(
            DLib.PLAYER,
            "Fine, fine..."
        );
        yield return new WaitWhile(() => DialogueManager.Instance.IsTypingActive());
        
        DialogueManager.Instance.HideDialogueUI();
    }
    #endregion
}
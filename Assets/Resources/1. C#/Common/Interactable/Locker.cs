using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class Locker : MonoBehaviour, IInteractable
{
    private bool cache = false;

    public void Interact(){
        // if(!PlayerMovement.Instance.canMove) return;
        if(GameManager.Instance.isEnded) return;

        cache ^= true;
        GetComponent<InteractableObject>().SetPrompt((!cache) ? "Hide" : "Come out");
        // PlayerMovement.Instance.HideInLocker(this.transform.position, cache);
    }
}
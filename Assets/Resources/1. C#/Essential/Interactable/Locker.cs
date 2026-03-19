using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class Locker : MonoBehaviour, IInteractable
{
    private bool cache = false;

    public void Interact(){
        cache ^= true;
        PlayerMovement.Instance.HideInLocker(this.transform.position, cache);
    }
}
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InteractableObject))]
public class GrandFatherClock : MonoBehaviour, IInteractable
{
    private Hanako hanako;

    public void Interact(){
        if(hanako == null){
            if(Hanako.Instance != null) hanako = Hanako.Instance;
            else return;
        }

        hanako.Taunt(this.transform);
    }
}
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public static PlayerInteraction Instance {get; private set;}

    [Header("REQUIRED ITEMS")]
    public bool hasTotem;
    public bool hasDrink;
    public bool hasMoney;

    [Header("DEBUG")]
    public bool skipMinigame;
    public bool ignoreDialogue;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }
}
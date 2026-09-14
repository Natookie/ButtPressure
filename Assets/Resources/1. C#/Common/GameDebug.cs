using UnityEngine;

/// <summary>
/// Used for debugging only
/// </summary>
public class GameDebug : MonoBehaviour
{
    public static GameDebug Instance {get; private set;}

    public bool skipMinigame = false;
    public bool ignoreDialogue = false;

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
        #if !UNITY_EDITOR
        doCutscene = true;
        skipMinigame = false;
        ignoreDialogue = false;
        #endif
    }
}

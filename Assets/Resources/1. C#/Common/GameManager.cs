using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    [Header("UI")]
    [SerializeField] private GameObject pooMeter;
    [SerializeField] private GameObject locationUI;
    [SerializeField] private GameObject objectiveUI;
    [SerializeField] private EndgameUI endgameUI;

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

        ClassroomCutscene.Instance.DoCutscene();

        AudioManager.Instance.PlayMusic(mainMusicAudioClip);
    }

    void Update(){
        if(keyboard != null && keyboard.spaceKey.isPressed) DialogueManager.Instance.SetFastForward(true);
        else DialogueManager.Instance.SetFastForward(false);
    }

    public void EnableEssentialUI(){
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
        Player.Instance.EnableInput = false;
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
}
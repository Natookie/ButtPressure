using UnityEngine;
using Nova;

public class ObstacleUI : MonoBehaviour
{
    [Header("UI REFERENCES")]
    [SerializeField] private TextBlock hanakoDistance;
    [SerializeField] private TextBlock tableCount;
    [SerializeField] private UIBlock2D progressFill;
    [SerializeField] private Transform playerPos;

    [Header("MINIGAME SETTINGS")]
    [SerializeField] private int totalTables = 4;
    [SerializeField] private int pressesPerTable = 3;
    
    private int currentTable = 0;
    private int currentPressesInTable = 0;
    
    void Start(){
        currentTable = 0;
        currentPressesInTable = 0;
        UpdateUI();
    }
    
    void Update(){
        HandleDistance();
        HandleTableCount();
        HandleTableProgress();
    }
    
    void HandleDistance(){
        Hanako hanako = Hanako.Instance;
        Transform hanakoPos = hanako.gameObject.transform;
        float xDiff = Mathf.Abs(hanakoPos.position.x - playerPos.position.x);
        hanakoDistance.Text = $"Hanako is {xDiff:F1}m away from you";
    }
    
    void HandleTableCount(){
        int diff = totalTables - currentTable;
        string pre = (diff == 1) ? "" : "s";
        tableCount.Text = $"<color=#4F6F52>{diff}</color> table{pre} left";
    }
    
    void HandleTableProgress(){
        float progress = (float)currentPressesInTable / pressesPerTable;
        progressFill.Size.X.Percent = progress;
    }
    
    public void AddProgress(int totalCorrectPresses){
        currentTable = totalCorrectPresses / pressesPerTable;
        currentPressesInTable = totalCorrectPresses % pressesPerTable;
        
        if(currentTable > totalTables){
            currentTable = totalTables;
            currentPressesInTable = 0;
        }
        
        UpdateUI();
    }
    
    public void AddTableCount() => UpdateUI();
    
    void UpdateUI(){
        HandleTableCount();
        HandleTableProgress();
    }
    
    public void ResetUI(){
        currentTable = 0;
        currentPressesInTable = 0;
        UpdateUI();
    }
}
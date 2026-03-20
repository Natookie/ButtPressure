using UnityEngine;
using Nova;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance {get; private set;}

    [Header("UI REFERENCES")]
    [SerializeField] private TextBlock objectiveText;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    public void SetObjective(string obj) => objectiveText.Text = obj;
}
using UnityEngine;
using Nova;

public class LocationUI : MonoBehaviour
{
    public static LocationUI Instance {get; private set;}

    [Header("UI REFERENCES")]
    [SerializeField] private TextBlock currentLocation;

    void Awake(){
        if(Instance == null) Instance = this;
        else{
            Destroy(gameObject);
            return;
        }
    }

    public void SetLocation(string loc) => currentLocation.Text = loc;
}
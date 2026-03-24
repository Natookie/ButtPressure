using UnityEngine;
using Nova;

public class PooMeter : MonoBehaviour
{
    [Header("TWEAKS")]
    [SerializeField] private GameObject pooContainer;
    [SerializeField] private float pooFill;
    [SerializeField] private float pooFillTarget;
    [Space(10)]
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    [Header("PENDULUM SETTINGS")]
    [SerializeField] private float pendulumSpeed = 2f;
    [SerializeField] private float pendulumAngle = 15f;

    [Header("UI REFERENCES")]
    [SerializeField] private UIBlock2D pooFillBlock;
    [SerializeField] private UIBlock2D pooIcon;

    [HideInInspector] public float pooMultiplier = 1f;

    private float pendulumTimer;
    private GameManager gm;

    void Start(){
        gm = GameManager.Instance;
        pooFill = 0f;
    }

    void Update(){
        if(!gm.isInitialized) return;
        
        CheckPoo();
        HandlePooFill();
    }

    void CheckPoo(){
        if(pooFill >= pooFillTarget) GameManager.Instance.EndGame(1);
    }

    public void UpdateMultiplier(){
        float remainingFill = pooFillTarget - pooFill;
        float remainingPercent = remainingFill / pooFillTarget;
        float targetTime = Mathf.Lerp(0.5f, 2f, remainingPercent);
        
        pooMultiplier = remainingFill / targetTime;
        pooMultiplier = Mathf.Clamp(pooMultiplier, 0.1f, 200f);
    }

    void HandlePooFill(){
        pooFill += Time.deltaTime * pooMultiplier;
        pooFill = Mathf.Clamp(pooFill, 0f, pooFillTarget);

        pooFillBlock.Size.X.Percent = pooFill / pooFillTarget;
        
        float t = pooFill / pooFillTarget;
        pooFillBlock.Color = Color.Lerp(endColor, startColor, t);

        pendulumTimer += Time.deltaTime * pendulumSpeed;
        float rotation = Mathf.Sin(pendulumTimer) * pendulumAngle;
        pooIcon.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}
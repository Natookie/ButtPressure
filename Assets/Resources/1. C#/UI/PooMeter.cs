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

    private float pendulumTimer;
    private GameManager gm;

    void Start(){
        gm = GameManager.Instance;
        pooFill = 0f;

        this.gameObject.SetActive(false);
    }

    void Update(){
        CheckPoo();
        HandlePooFill();
    }

    void CheckPoo(){
        if(pooFill > pooFillTarget) GameManager.Instance.EndGame();
    }

    void HandlePooFill(){
        pooFill += Time.deltaTime;
        pooFill = Mathf.Clamp(pooFill, 0f, pooFillTarget);
        
        pooFillBlock.Size.X.Percent = (pooFill / 100f);
        
        float t = pooFill / 100f;
        pooFillBlock.Color = Color.Lerp(endColor, startColor, t);

        pendulumTimer += Time.deltaTime * pendulumSpeed;
        float rotation = Mathf.Sin(pendulumTimer) * pendulumAngle;
        pooIcon.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}
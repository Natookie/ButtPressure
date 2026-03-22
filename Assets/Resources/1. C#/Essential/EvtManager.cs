using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EvtManager : MonoBehaviour
{
    [Header("EVENT SETTINGS")]
    [SerializeField] private bool oneTime = true;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private string targetTag = "Player";
    [HideInInspector] public bool allowTrigger = false;
    
    [Header("ACTIONS")]
    public bool teleportHanako;
    [SerializeField] private Transform teleportTarget;
    
    [Header("OTHER ACTIONS")]
    [SerializeField] private UnityEngine.Events.UnityEvent onTriggerEvents;
    
    private BoxCollider2D triggerCollider;
    private bool hasTriggered;

    void Start(){
        triggerCollider = GetComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other){
        if(!allowTrigger) return;
        if(hasTriggered && oneTime) return;
        
        if(((1 << other.gameObject.layer) & targetLayer) != 0){
            if(string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))
                ExecuteEvent();
        }
    }

    void ExecuteEvent(){
        Hanako hanako = Hanako.Instance;
        if(teleportHanako && hanako != null && teleportTarget != null)
            hanako.TeleportTo(teleportTarget.position);
        
        onTriggerEvents?.Invoke();
        
        if(oneTime) hasTriggered = true;
    }
}
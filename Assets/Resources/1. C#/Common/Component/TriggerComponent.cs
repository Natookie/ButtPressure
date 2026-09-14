using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerComponent : MonoBehaviour
{
    public UnityEvent PlayerEnter;
    public UnityEvent PlayerExit;

    [Header("Detection Settings")]
    [Tooltip("Tag used to identify the player object")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("If true, the event can only fire once")]
    [SerializeField] private bool triggerOnce = false;

    private bool hasTriggered = false;

    private void Reset()
    {
        // Make sure the collider is set up as a trigger by default
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag(playerTag))
        {
            hasTriggered = true;
            PlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (triggerOnce && hasTriggered) return;

        if (other.CompareTag(playerTag))
        {
            PlayerExit?.Invoke();
        }
    }
}

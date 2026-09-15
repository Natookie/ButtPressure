using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interact Settings")]
    [SerializeField] private string interactableTagName = "Interactable";

    [Tooltip("The closest interactable for interacting")]
    public InteractableComponent CurrentTarget { get; private set; }

    public bool CanInteract
    {
        set
        {
            canInteract = value;
            if (canInteract && CurrentTarget) visualCue.ShowPrompt(CurrentTarget);
            else visualCue.HidePrompt();
        }
    }

    [SerializeField] private VisualCue visualCue;
    private bool canInteract = true;
    private readonly List<InteractableComponent> interactable_list = new List<InteractableComponent>();
    private InteractableComponent pendingInteractableDeletion;

    private void Start()
    {
        if (VisualCue.Instance) visualCue = VisualCue.Instance;
    }

    private void Update()
    {
        if (!canInteract) return;
        UpdateCurrentTarget();
        if (Keyboard.current.eKey.wasPressedThisFrame) TryInteract();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOnInteractableTag(other.gameObject)) return;
        var interactable = other.GetComponentInParent<InteractableComponent>();
        if (interactable != null && !interactable_list.Contains(interactable))
        {
            interactable_list.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsOnInteractableTag(other.gameObject)) return;
        var interactable = other.GetComponentInParent<InteractableComponent>();
        if (interactable != null)
        {
            interactable_list.Remove(interactable);
            if (CurrentTarget == interactable) CurrentTarget = null;
        }
    }

    /// <summary>
    /// Picks the closest, currently-interactable as the active target.
    /// </summary>
    private void UpdateCurrentTarget()
    {
        // Clean up any destroyed/disabled objects first.
        interactable_list.RemoveAll(c => c == null || !((MonoBehaviour)c) || c.Equals(null));
        // Find the closest interactable
        pendingInteractableDeletion = null;
        InteractableComponent closest = null;
        float closestDistance = float.MaxValue;
        foreach (InteractableComponent interactable in interactable_list)
        {
            // Check is valid
            if (interactable == null || !interactable.CanInteract) continue;
            // Mark for deletion if not active
            if (!interactable.isActiveAndEnabled) pendingInteractableDeletion = interactable;
            // Calculate distance
            var mono = interactable as MonoBehaviour;
            if (mono == null) continue;
            float distance = Vector3.SqrMagnitude(mono.transform.position - transform.position);
            // Update if it's closer
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = interactable;
            }
        }
        // Set new closest
        CurrentTarget = closest;
        if (CurrentTarget) visualCue.ShowPrompt(closest);
        else visualCue.HidePrompt();
        // Do deletion
        interactable_list.Remove(pendingInteractableDeletion);
    }

    private void TryInteract()
    {
        if (CurrentTarget == null || !CurrentTarget.CanInteract) return;
        CurrentTarget.Interact();
    }

    private bool IsOnInteractableTag(GameObject obj) {return obj.CompareTag(interactableTagName);}
}
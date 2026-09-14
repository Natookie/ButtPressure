using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Act as a bridge between interact triggerer and interactable object
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InteractableComponent : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private MonoBehaviour interactableTargetBehaviour;

    private IInteractable interactableTarget;

    [Header("INTERACTION SETTINGS")]
    public bool CanInteract = true;
    [SerializeField] private string interactionPrompt = "Press E to interact";

    public string Prompt => interactionPrompt;

    private void Start()
    {
        interactableTarget = interactableTargetBehaviour as IInteractable;
        Debug.Assert(
            !interactableTarget.IsUnityNull(),
            "interactableTargetBehaviour must implement IInteractable"
        );
    }

    public void Interact()
    {
        if (CanInteract && interactableTarget != null)
            interactableTarget.Interact();
    }

    public void SetPrompt(string newPrompt) {interactionPrompt = newPrompt;}
}
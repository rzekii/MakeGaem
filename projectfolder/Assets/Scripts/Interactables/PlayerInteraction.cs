using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private TextMeshProUGUI interactionPrompt;

    private IInteractable closestInteractable;

    private void Start()
    {
        if (interactionPrompt == null)
        {
            Debug.LogError("❌ Interaction prompt UI is not assigned in the Inspector!");
            return;
        }

        interactionPrompt.gameObject.SetActive(false);
        StartCoroutine(DetectInteractablesRoutine());
    }

    private IEnumerator DetectInteractablesRoutine()
    {
        while (true)
        {
            DetectClosestInteractable();
            UpdateInteractionPrompt();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void DetectClosestInteractable()
    {
        Collider[] interactables = Physics.OverlapSphere(transform.position, interactionRadius, interactableLayer);
        float closestDistance = Mathf.Infinity;
        closestInteractable = null;

        foreach (Collider collider in interactables)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }
    }

    private void UpdateInteractionPrompt()
    {
        if (closestInteractable != null && CanInteractWith(closestInteractable))
        {
            interactionPrompt.gameObject.SetActive(true);
            interactionPrompt.text = "[E] " + closestInteractable.GetInteractionName();
        }
        else
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    private bool CanInteractWith(IInteractable interactable)
    {
        if (interactable is GearPickup gearPickup)
        {
            if (gearPickup.IsPickedUp)
                return false;

            if (PlayerInventory.Instance != null && PlayerInventory.Instance.IsWeaponEquipped())
                return false;
        }
        return true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && closestInteractable != null)
        {
            closestInteractable.Interact();
        }
    }
}

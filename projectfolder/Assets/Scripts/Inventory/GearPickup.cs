using UnityEngine;

public class GearPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private Gear gearItem;
    public bool IsPickedUp { get; private set; } = false;

    private void Awake()
    {
        if (gearItem == null)
        {
            Debug.LogError("❌ Gear ScriptableObject is missing in GearPickup!");
        }
        else
        {
            gearItem.AssignIcon();
        }
    }

    public void Interact()
    {
        if (IsPickedUp || gearItem == null)
        {
            Debug.LogWarning($"⚠ {gearItem?.gearName ?? "Unknown Gear"} already picked up or missing.");
            return;
        }

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddGear(gearItem);
        }
        else
        {
            Debug.LogError("❌ PlayerInventory.Instance is null! Gear not added.");
        }

        IsPickedUp = true;
        gameObject.SetActive(false);
    }

    public string GetInteractionName()
    {
        return gearItem != null ? $"Pick Up {gearItem.gearName}" : "Pick Up [Missing Gear]";
    }
}

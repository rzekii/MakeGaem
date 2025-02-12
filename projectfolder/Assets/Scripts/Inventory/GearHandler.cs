using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GearHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image gearIconImage;
    private Gear assignedGear;

    public void SetGear(Gear gear)
    {
        if (gear == null)
        {
            ClearSlot();
            return;
        }

        assignedGear = gear;

        if (assignedGear.gearIcon == null)
        {
            assignedGear.AssignIcon();
        }

        UpdateIcon(assignedGear.gearIcon);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (assignedGear == null) return;

        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("❌ PlayerInventory instance is missing!");
            return;
        }

        if (PlayerInventory.Instance.IsGearEquipped(assignedGear))
        {
            // ✅ Unequip the item if it's currently equipped
            Debug.Log($"🔄 Unequipping {assignedGear.gearName}...");
            PlayerInventory.Instance.UnequipGear(assignedGear.gearType);
            ClearSlot();
        }
        else
        {
            Debug.Log($"🛠 Attempting to equip {assignedGear.gearName}...");

            // ✅ Assign the item to the correct UI slot before equipping
            AssignGearToSlot(assignedGear);

            // ✅ Check if the gear is now in an equipment slot
            if (PlayerInventory.Instance.IsGearInEquipmentSlot(assignedGear))
            {
                PlayerInventory.Instance.EquipGear(assignedGear);
            }
            else
            {
                Debug.LogWarning($"⚠ {assignedGear.gearName} must be moved to an equipment slot to be equipped.");
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (assignedGear != null && Tooltip.Instance != null)
        {
            Tooltip.Instance.ShowTooltip(assignedGear, Input.mousePosition);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance?.HideTooltip();
    }

    public void ClearSlot()
    {
        assignedGear = null;
        UpdateIcon(null);
    }

    private void UpdateIcon(Sprite icon)
    {
        if (gearIconImage == null) return;

        gearIconImage.sprite = icon;
        gearIconImage.enabled = icon != null;
    }

    // ✅ Assigns Gear to the Correct Equipment Slot
    private void AssignGearToSlot(Gear gear)
    {
        if (gear == null) return;

        Debug.Log($"🔍 Assigning {gear.gearName} to its correct slot...");

        switch (gear.gearType)
        {
            case GearType.Weapon:
                PlayerInventory.Instance.swordSlot.sprite = gear.gearIcon;
                break;
            case GearType.Head:
                PlayerInventory.Instance.headSlot.sprite = gear.gearIcon;
                break;
            case GearType.Chest:
                PlayerInventory.Instance.chestSlot.sprite = gear.gearIcon;
                break;
            case GearType.Boots:
                PlayerInventory.Instance.bootsSlot.sprite = gear.gearIcon;
                break;
            default:
                Debug.LogWarning($"⚠ No slot found for {gear.gearName}!");
                break;
        }
    }

    // ✅ Retrieves Assigned Gear
    public Gear GetGear()
    {
        return assignedGear;
    }
}

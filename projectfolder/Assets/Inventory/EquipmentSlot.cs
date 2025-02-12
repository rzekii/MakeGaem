using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private GearType slotType; // ✅ The type of gear this slot can hold
    [SerializeField] private Image slotImage;

    private Gear assignedGear;

    private void Awake()
    {
        if (slotImage == null)
        {
            slotImage = GetComponent<Image>();
        }
    }

    // ✅ Handles when an item is dropped onto this equipment slot
    public void OnDrop(PointerEventData eventData)
    {
        GearHandler draggedGearHandler = eventData.pointerDrag?.GetComponent<GearHandler>();

        if (draggedGearHandler == null)
        {
            Debug.LogWarning("⚠ No GearHandler found on dropped object!");
            return;
        }

        Gear gear = draggedGearHandler.GetGear();

        if (gear == null)
        {
            Debug.LogWarning("⚠ Dropped item does not contain valid gear!");
            return;
        }

        // ✅ Ensure the slot type matches the gear type
        if (gear.gearType != slotType)
        {
            Debug.LogWarning($"⚠ {gear.gearName} cannot be placed in {slotType} slot!");
            return;
        }

        EquipGear(gear);
    }

    // ✅ Handles equipping gear in the UI and updating the inventory
    public void EquipGear(Gear gear)
    {
        if (gear == null)
        {
            Debug.LogError("❌ Cannot equip null gear!");
            return;
        }

        // ✅ Check if there's already an item in the slot
        if (assignedGear != null)
        {
            UnequipGear();
        }

        assignedGear = gear;
        slotImage.sprite = gear.gearIcon;
        slotImage.enabled = true;

        // ✅ Ensure PlayerInventory tracks this equipped gear
        PlayerInventory.Instance.EquipGear(gear);

        Debug.Log($"✅ Equipped {gear.gearName} in {slotType} slot.");
    }

    // ✅ Handles unequipping gear from the slot
    public void UnequipGear()
    {
        if (assignedGear == null)
        {
            Debug.LogWarning("⚠ No gear equipped in this slot.");
            return;
        }

        // ✅ Remove gear from PlayerInventory
        PlayerInventory.Instance.UnequipGear(slotType);

        // ✅ Clear the slot
        assignedGear = null;
        slotImage.sprite = null;
        slotImage.enabled = false;

        Debug.Log($"✅ Unequipped gear from {slotType} slot.");
    }

    // ✅ Returns the currently assigned gear
    public Gear GetEquippedGear()
    {
        return assignedGear;
    }
}

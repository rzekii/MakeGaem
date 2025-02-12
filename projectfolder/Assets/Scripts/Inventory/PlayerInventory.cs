using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Inventory UI Elements")]
    [SerializeField] private GameObject bagPanel;
    [SerializeField] private GameObject characterStatsPanel;
    [SerializeField] private Transform inventoryGrid;
    [SerializeField] private List<GearHandler> inventorySlots;

    [Header("Equipment Slots")]
    public Image headSlot;
    public Image chestSlot;
    public Image bootsSlot;
    public Image swordSlot;

    [Header("Player References")]
    [SerializeField] private GameObject playerModel;
    [SerializeField] private Transform weaponAttachment;

    private List<Gear> inventory = new List<Gear>();
    private PlayerStats playerStats;
    private PlayerStatsUI playerStatsUI;

    private Gear equippedWeapon = null;
    private bool isWeaponEquipped = false;
    private bool isHoveringOverSwordSlot = false;
    public Dictionary<GearType, Gear> equippedGear = new Dictionary<GearType, Gear>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ValidateReferences();
    }

    private void ValidateReferences()
    {
        playerStats = playerModel?.GetComponent<PlayerStats>();

        if (playerStats == null || swordSlot == null)
        {
            Debug.LogError("❌ Missing player components. Check Inspector assignments.");
        }

        playerStatsUI = FindObjectOfType<PlayerStatsUI>();
        if (playerStatsUI == null)
        {
            Debug.LogError("❌ PlayerStatsUI not found in the scene!");
        }
    }

    private void Start()
    {
        if (bagPanel != null) bagPanel.SetActive(false);
        if (characterStatsPanel != null) characterStatsPanel.SetActive(false);

        playerStats?.InitializeStats();
        playerStatsUI?.UpdateStatsUI();
        UpdateInventoryUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (isWeaponEquipped && swordSlot != null && Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Input.mousePosition;
            RectTransform swordSlotRect = swordSlot.rectTransform;

            if (RectTransformUtility.RectangleContainsScreenPoint(swordSlotRect, mousePos))
            {
                TryUnequipWeapon();
            }
        }

        HandleSwordSlotHover();
    }

    private void HandleSwordSlotHover()
    {
        if (swordSlot == null || equippedWeapon == null) return;

        Vector2 mousePos = Input.mousePosition;
        RectTransform swordSlotRect = swordSlot.rectTransform;

        if (RectTransformUtility.RectangleContainsScreenPoint(swordSlotRect, mousePos))
        {
            if (!isHoveringOverSwordSlot)
            {
                isHoveringOverSwordSlot = true;
                Tooltip.Instance?.ShowTooltip(equippedWeapon, mousePos + new Vector2(10, -10));
            }
        }
        else if (isHoveringOverSwordSlot)
        {
            isHoveringOverSwordSlot = false;
            Tooltip.Instance?.HideTooltip();
        }
    }

    private void ToggleInventory()
    {
        if (bagPanel == null || characterStatsPanel == null) return;

        bool isOpen = !bagPanel.activeSelf;
        bagPanel.SetActive(isOpen);
        characterStatsPanel.SetActive(isOpen);

        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;

        if (isOpen)
        {
            UpdateInventoryUI();
            playerStatsUI?.UpdateStatsUI();
        }
    }

    public void AddGear(Gear newGear)
    {
        if (newGear == null)
        {
            Debug.LogWarning("⚠ Attempted to add a null gear to the inventory.");
            return;
        }

        if (inventory.Contains(newGear))
        {
            Debug.LogWarning($"⚠ {newGear.gearName} is already in the inventory.");
            return;
        }

        if (inventory.Count >= inventorySlots.Count)
        {
            Debug.LogWarning("⚠ Inventory is full. Cannot add more gear.");
            return;
        }

        inventory.Add(newGear);
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i < inventory.Count)
            {
                if (inventory[i] == equippedWeapon)
                {
                    inventorySlots[i].ClearSlot();
                }
                else
                {
                    inventorySlots[i].SetGear(inventory[i]);
                }
            }
            else
            {
                inventorySlots[i].ClearSlot();
            }
        }
    }

    public void EquipGear(Gear gear)
    {
        if (gear == null)
        {
            Debug.LogError("❌ Attempted to equip null gear!");
            return;
        }

        if (!IsGearInEquipmentSlot(gear))
        {
            Debug.LogWarning($"⚠ {gear.gearName} must be placed in an equipment slot to be equipped.");
            return;
        }

        if (equippedGear.ContainsKey(gear.gearType))
        {
            UnequipGear(gear.gearType);
        }

        equippedGear[gear.gearType] = gear;
        playerStats?.ApplyStatBoost(gear);
        playerStatsUI?.UpdateStatsUI();

        if (gear.gearType == GearType.Weapon)
        {
            equippedWeapon = gear;
            isWeaponEquipped = true;
            MoveWeaponToSwordSlot(gear);

            // ✅ Updates weapon name only when it's equipped
            UpdateStatsText(gear.gearName);
        }

        UpdateInventoryUI();
    }

    public void UnequipGear(GearType gearType)
    {
        if (!equippedGear.ContainsKey(gearType))
        {
            Debug.LogWarning("⚠ Attempted to unequip non-equipped gear.");
            return;
        }

        Gear gear = equippedGear[gearType];
        equippedGear.Remove(gearType);
        playerStats?.RemoveStatBoost(gear);
        playerStatsUI?.UpdateStatsUI();

        if (gearType == GearType.Weapon)
        {
            isWeaponEquipped = false;
            equippedWeapon = null;
            ClearSwordSlot();

            // ✅ Reset StatsText when unequipping
            UpdateStatsText("No Weapon Equipped");
        }

        UpdateInventoryUI();
    }

    private void MoveWeaponToSwordSlot(Gear gear)
    {
        if (swordSlot != null)
        {
            swordSlot.sprite = gear.gearIcon;
            swordSlot.enabled = true;
        }
    }

    private void ClearSwordSlot()
    {
        if (swordSlot != null)
        {
            swordSlot.sprite = null;
            swordSlot.enabled = false;
        }
    }

    private void TryUnequipWeapon()
    {
        if (isWeaponEquipped)
        {
            UnequipGear(GearType.Weapon);
        }
    }

    public bool IsGearEquipped(Gear gear)
    {
        return equippedGear.TryGetValue(gear.gearType, out Gear equipped) && equipped == gear;
    }

    public bool IsGearInEquipmentSlot(Gear gear)
    {
        if (gear == null) return false;

        if (swordSlot == null || headSlot == null || chestSlot == null || bootsSlot == null)
        {
            Debug.LogError("❌ One or more equipment UI slots are not assigned in the Inspector!");
            return false;
        }

        return (gear.gearType == GearType.Weapon && swordSlot.sprite != null && swordSlot.sprite == gear.gearIcon) ||
               (gear.gearType == GearType.Head && headSlot.sprite != null && headSlot.sprite == gear.gearIcon) ||
               (gear.gearType == GearType.Chest && chestSlot.sprite != null && chestSlot.sprite == gear.gearIcon) ||
               (gear.gearType == GearType.Boots && bootsSlot.sprite != null && bootsSlot.sprite == gear.gearIcon);
    }

    private void UpdateStatsText(string weaponName)
    {
        if (playerStatsUI != null)
        {
            playerStatsUI.SetWeaponName(weaponName);
        }
    }

    public bool IsWeaponEquipped() => isWeaponEquipped;
    public Gear GetEquippedWeapon() => equippedWeapon;
}

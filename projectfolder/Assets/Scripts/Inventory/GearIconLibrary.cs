using UnityEngine;
using System.Collections.Generic;

public class GearIconLibrary : MonoBehaviour
{
    public static GearIconLibrary Instance;

    [Header("Gear Icons")]
    [SerializeField] private Sprite weaponIcon;
    [SerializeField] private Sprite headIcon;
    [SerializeField] private Sprite chestIcon;
    [SerializeField] private Sprite bootsIcon;
    [SerializeField] private Sprite potionIcon;
    [SerializeField] private Sprite questItemIcon;
    [SerializeField] private Sprite defaultIcon;

    private Dictionary<GearType, Sprite> _iconDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeDictionary();
            Debug.Log("✅ GearIconLibrary initialized.");

            // Uncomment if you want it to persist across scenes
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Debug.LogWarning("⚠ Duplicate GearIconLibrary found. Destroying the duplicate.");
            Destroy(gameObject);
        }
    }

    private void InitializeDictionary()
    {
        _iconDictionary = new Dictionary<GearType, Sprite>
        {
            { GearType.Weapon, weaponIcon },
            { GearType.Head, headIcon },
            { GearType.Chest, chestIcon },
            { GearType.Boots, bootsIcon },
            { GearType.Potion, potionIcon },
            { GearType.QuestItem, questItemIcon }
        };

        // Check if any essential icons are missing in the inspector
        foreach (var kvp in _iconDictionary)
        {
            if (kvp.Value == null)
            {
                Debug.LogWarning($"⚠ Missing icon assignment for {kvp.Key} in the Inspector.");
            }
        }

        // Check for default icon assignment
        if (defaultIcon == null)
        {
            Debug.LogWarning("⚠ Default icon is not assigned in the Inspector. Missing icons will not display correctly.");
        }
    }

    public Sprite GetIcon(GearType gearType)
    {
        if (_iconDictionary.TryGetValue(gearType, out Sprite icon))
        {
            return icon;
        }
        else
        {
            Debug.LogWarning($"⚠ No icon found for {gearType} in GearIconLibrary. Using default icon.");
            return defaultIcon != null ? defaultIcon : null;  // Ensure no null reference
        }
    }
}

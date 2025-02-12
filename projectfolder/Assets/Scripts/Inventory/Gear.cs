using UnityEngine;

public enum WeaponRarity { Common, Uncommon, Rare, Epic, Legendary }

[CreateAssetMenu(fileName = "NewGear", menuName = "Inventory/Gear")]
public class Gear : ScriptableObject
{
    [Header("General Info")]
    public string gearName;
    public GearType gearType;
    public Sprite gearIcon;

    [Header("Base Stats")]
    public int baseDamage;
    public int constitutionBonus;
    public int strengthBonus;
    public int agilityBonus;
    public int intelligenceBonus;
    public int armorBonus;

    [Header("Rarity")]
    public WeaponRarity rarity;

    [Header("Visuals")]
    public GameObject gearPrefab;

    public void AssignIcon()
    {
        if (gearIcon == null && GearIconLibrary.Instance != null)
        {
            gearIcon = GearIconLibrary.Instance.GetIcon(gearType);
            Debug.Log($"🖼️ Icon assigned for {gearName}: {gearIcon?.name ?? "None"}");
        }
    }

    public void RandomizeStats()
    {
        rarity = DetermineWeaponRarity();
        ApplyRarityStats();
        AssignIcon();
        Debug.Log($"🎲 Randomized {gearName} | Damage: {baseDamage}, STR: {strengthBonus}, CON: {constitutionBonus}, AGI: {agilityBonus}, Rarity: {rarity}");
    }

    private void ApplyRarityStats()
    {
        int rarityMultiplier = (int)rarity;
        int minDamage = 3 + rarityMultiplier;
        int maxDamage = 8 + rarityMultiplier * 2;

        baseDamage = Random.Range(minDamage, maxDamage + 1);
        strengthBonus = Random.Range(1 + rarityMultiplier, 4 + rarityMultiplier);
        constitutionBonus = Random.Range(0 + rarityMultiplier, 4 + rarityMultiplier);
        agilityBonus = Random.Range(0 + rarityMultiplier, 2 + rarityMultiplier);
    }

    private WeaponRarity DetermineWeaponRarity()
    {
        int roll = Random.Range(1, 101);
        Debug.Log($"🎲 Rarity Roll: {roll}");

        return roll switch
        {
            <= 60 => WeaponRarity.Common,
            <= 77 => WeaponRarity.Uncommon,
            <= 90 => WeaponRarity.Rare,
            <= 99 => WeaponRarity.Epic,
            _ => WeaponRarity.Legendary,
        };
    }
}

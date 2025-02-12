using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Gear weaponData;  // Assign your weapon ScriptableObject in the Inspector

    private void Start()
    {
        if (weaponData != null)
        {
            GenerateAndSpawnWeapon(weaponData);
        }
        else
        {
            Debug.LogError("❌ Weapon data not assigned in WeaponManager!");
        }
    }

    public void GenerateAndSpawnWeapon(Gear weapon)
    {
        if (weapon == null)
        {
            Debug.LogError("❌ Weapon data is null!");
            return;
        }

        // Step 1: Determine weapon rarity
        weapon.rarity = DetermineWeaponRarity();

        // Step 2: Apply stats based on rarity
        ApplyRarityStats(weapon);

        // Step 3: Log the generated weapon details
        Debug.Log($"🗡️ Generated Weapon: {weapon.gearName} | Damage: {weapon.baseDamage} | STR: {weapon.strengthBonus} | CON: {weapon.constitutionBonus} | AGI: {weapon.agilityBonus} | INT: {weapon.intelligenceBonus} | Rarity: {weapon.rarity}");
    }

    private WeaponRarity DetermineWeaponRarity()
    {
        int roll = Random.Range(1, 101);
        Debug.Log($"🎲 Rarity Roll: {roll}");

        if (roll <= 60)
            return WeaponRarity.Common;     // 60% chance
        else if (roll <= 77)
            return WeaponRarity.Uncommon;   // 17% chance
        else if (roll <= 90)
            return WeaponRarity.Rare;       // 13% chance
        else if (roll <= 99)
            return WeaponRarity.Epic;       // 9% chance
        else
            return WeaponRarity.Legendary;  // 1% chance
    }

    private void ApplyRarityStats(Gear weapon)
    {
        int rarityMultiplier = (int)weapon.rarity;

        // **Base Damage Scaling**: +4 per rarity level
        int baseDamageMin = 5 + (rarityMultiplier * 4);
        int baseDamageMax = 8 + (rarityMultiplier * 4);
        weapon.baseDamage = Random.Range(baseDamageMin, baseDamageMax + 1); // +1 to include max value

        // **Stat Scaling**: Other stats scale independently of base damage
        weapon.strengthBonus = Random.Range(1 + rarityMultiplier, 4 + rarityMultiplier);
        weapon.constitutionBonus = Random.Range(0 + rarityMultiplier, 4 + rarityMultiplier);
        weapon.agilityBonus = Random.Range(0 + rarityMultiplier, 2 + rarityMultiplier);

        // **Intelligence Bonus**: Only for Epic and Legendary weapons
        weapon.intelligenceBonus = (weapon.rarity == WeaponRarity.Epic || weapon.rarity == WeaponRarity.Legendary)
            ? Random.Range(1 + rarityMultiplier, 3 + rarityMultiplier)
            : 0;

        // **Apply Strength Bonus to Damage**
        weapon.baseDamage += weapon.strengthBonus;

        Debug.Log($"💥 Final Weapon Damage with STR Bonus: {weapon.baseDamage} (Includes STR: +{weapon.strengthBonus})");
    }
}
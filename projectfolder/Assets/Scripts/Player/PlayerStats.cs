using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Player Stats")]
    [SerializeField] private int baseMaxHealth = 40;  // Starting health value
    [SerializeField] private int baseMaxStamina = 40;  // Starting stamina value
    [SerializeField] private int baseConstitution = 5;
    [SerializeField] private int baseStrength = 5;
    [SerializeField] private int baseAgility = 5;
    [SerializeField] private int baseIntelligence = 3;
    [SerializeField] private int baseArmor = 1;
    [SerializeField] private int currentHealth = 0;
    [SerializeField] private int currentStamina =1;

    private int maxHealth;
    private int maxStamina;
    private int constitution;
    private int strength;
    private int agility;
    private int intelligence;
    private int armor;


    private Gear equippedWeapon;

    private void Awake()
    {
        InitializeStats();
    }

    public void InitializeStats()
    {
        // Initialize base stats
        constitution = baseConstitution;
        strength = baseStrength;
        agility = baseAgility;
        intelligence = baseIntelligence;
        armor = baseArmor;

        // Recalculate max health and stamina based on constitution and agility
        maxHealth = baseMaxHealth + (constitution * 3);  // Base health + Constitution * 3
        maxStamina = baseMaxStamina + (agility * 3);     // Base stamina + Agility * 3

        currentHealth = maxHealth; // Start at max health
        currentStamina = maxStamina; // Start at max stamina
    }

    public PlayerStatsData GetStatsData()
    {
        return new PlayerStatsData
        {
            CurrentHealth = currentHealth,
            MaxHealth = maxHealth,
            CurrentStamina = currentStamina,
            MaxStamina = maxStamina,
            Constitution = constitution,
            Strength = strength,
            Agility = agility,
            Intelligence = intelligence,
            Armor = armor,
            DamageReduction = CalculateDamageReduction(),
            WeaponInfo = equippedWeapon != null
                ? $"Weapon: {equippedWeapon.gearName} ({equippedWeapon.rarity})"
                : "Weapon: None"
        };
    }

    public float CalculateDamageReduction()
    {
        return armor * 0.02f;  // Example: Each armor point reduces damage by 2%
    }

    public void TakeDamage(int damage)
    {
        int reducedDamage = Mathf.RoundToInt(damage * (1 - CalculateDamageReduction()));
        currentHealth = Mathf.Max(currentHealth - reducedDamage, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("💀 Player has died!");
        // Add death handling here (e.g., respawn, game over, etc.)
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void RestoreStamina(int amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
    }

    public void ApplyStatBoost(Gear gear)
    {
        if (gear == null) return;

        Debug.Log($"🛡️ Equipping {gear.gearName}: Applying Stat Boosts");

        // Apply stat boosts from gear
        strength += gear.strengthBonus;
        constitution += gear.constitutionBonus;
        agility += gear.agilityBonus;
        intelligence += gear.intelligenceBonus;
        armor += gear.armorBonus;

        RecalculateStats();
    }

    public void RemoveStatBoost(Gear gear)
    {
        if (gear == null) return;

        Debug.Log($"❌ Unequipping {gear.gearName}: Removing Stat Boosts");

        strength = Mathf.Max(0, strength - gear.strengthBonus);
        constitution = Mathf.Max(0, constitution - gear.constitutionBonus);
        agility = Mathf.Max(0, agility - gear.agilityBonus);
        intelligence = Mathf.Max(0, intelligence - gear.intelligenceBonus);
        armor = Mathf.Max(0, armor - gear.armorBonus);

        RecalculateStats();
    }

    public void RecalculateStats()
    {
        Debug.Log("🔄 Recalculating Stats...");

        // Recalculate max health and max stamina based on the current stats
        int newMaxHealth = baseMaxHealth + (constitution * 3);  // Base health + Constitution * 3
        int newMaxStamina = baseMaxStamina + (agility * 3);     // Base stamina + Agility * 3

        if (newMaxHealth != maxHealth || newMaxStamina != maxStamina)
        {
            maxHealth = newMaxHealth;
            maxStamina = newMaxStamina;

            // Adjust current health to match max health if it's higher than max
            currentHealth = Mathf.Min(currentHealth + (constitution*3), maxHealth);   // Adjust current health if it's higher than max
            currentStamina = Mathf.Min(currentStamina + (agility*3), maxStamina); // Adjust stamina similarly

            // Optionally restore health/stamina to match max (if desired)
            // RestoreHealth(currentHealth);
            // RestoreStamina(currentStamina);
        }

        Debug.Log($"✅ New Max Health: {maxHealth}, Max Stamina: {maxStamina}");

        // Notify UI to update
        if (PlayerStatsUI.Instance != null)
        {
            PlayerStatsUI.Instance.UpdateStatsUI();
        }
    }
}

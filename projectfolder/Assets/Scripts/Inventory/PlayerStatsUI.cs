using UnityEngine;
using TMPro;
using System.Text;

public class PlayerStatsUI : MonoBehaviour
{
    public static PlayerStatsUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI statsText; // Assign in Inspector
    private PlayerStats playerStats;
    private StringBuilder statsBuilder;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure the UI persists across scenes
        }
        else
        {
            Debug.LogWarning("⚠️ Duplicate PlayerStatsUI instance detected. Destroying duplicate.");
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }

        InitializeComponents();
    }

    private void Start()
    {
        if (IsInitialized())
        {
            UpdateStatsUI(); // Initial update
        }
        else
        {
            Debug.LogError("❌ PlayerStats or StatsText is missing in Start!");
        }
    }

    private void InitializeComponents()
    {
        if (statsText == null)
        {
            Debug.LogError("❌ StatsText is not assigned in the Inspector!");
            return;
        }

        // Cache PlayerStats reference
        if (PlayerInventory.Instance != null)
        {
            playerStats = PlayerInventory.Instance.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerInventory.Instance is null. Falling back to FindObjectOfType.");
            PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
            if (inventory != null)
            {
                playerStats = inventory.GetComponent<PlayerStats>();
            }
        }

        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("❌ PlayerStats not found in the scene!");
            }
        }

        // Initialize StringBuilder
        statsBuilder = new StringBuilder();
    }

    public void UpdateStatsUI()
    {
        if (!IsInitialized())
        {
            Debug.LogError("❌ PlayerStats or StatsText missing in UpdateStatsUI!");
            return;
        }

        var statsData = playerStats.GetStatsData();

        // Use StringBuilder to construct the stats string
        statsBuilder.Clear(); // Clear previous content
        statsBuilder.AppendLine($"Health: {statsData.CurrentHealth}/{statsData.MaxHealth}");
        statsBuilder.AppendLine($"Stamina: {statsData.CurrentStamina}/{statsData.MaxStamina}");
        statsBuilder.AppendLine($"Constitution: {statsData.Constitution}");
        statsBuilder.AppendLine($"Strength: {statsData.Strength}");
        statsBuilder.AppendLine($"Agility: {statsData.Agility}");
        statsBuilder.AppendLine($"Intelligence: {statsData.Intelligence}");
        statsBuilder.AppendLine($"Armor: {statsData.Armor} ({statsData.DamageReduction * 100:F1}% Damage Reduction)");
        statsBuilder.AppendLine(statsData.WeaponInfo);

        // Update the UI text
        statsText.text = statsBuilder.ToString();
        Debug.Log("Player Stats UI Updated!");
    }

    public bool IsInitialized()
    {
        return playerStats != null && statsText != null;
    }

    public void SetWeaponName(string weaponName)
    {
        if (statsText == null)
        {
            Debug.LogError("❌ StatsText is not assigned!");
            return;
        }

        string currentText = statsText.text;
        int weaponIndex = currentText.IndexOf("Weapon: ");

        if (weaponIndex != -1)
        {
            string newText = currentText.Substring(0, weaponIndex) + $"Weapon: {weaponName}";
            statsText.text = newText;
        }
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private CanvasGroup canvasGroup; // ✅ For smooth fade-in/out

    private Gear currentGear;
    private RectTransform tooltipRectTransform;
    private Vector2 tooltipOffset = new Vector2(15f, -15f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("⚠ Duplicate Tooltip detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        if (tooltipPanel == null || tooltipText == null)
        {
            Debug.LogError("❌ Tooltip UI components are missing! Check inspector.");
        }

        tooltipRectTransform = tooltipPanel?.GetComponent<RectTransform>();

        if (canvasGroup == null)
        {
            canvasGroup = tooltipPanel?.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogWarning("⚠ No CanvasGroup found on Tooltip Panel. Fading will not work.");
            }
        }

        tooltipPanel?.SetActive(false);
    }

    public void ShowTooltip(Gear gear, Vector2 position)
    {
        if (gear == null || tooltipPanel == null || tooltipText == null) return;

        if (currentGear == gear && tooltipPanel.activeSelf) return;
        currentGear = gear;

        string rarityColor = GetRarityColor(gear.rarity);
        tooltipText.text = $"<b><color={rarityColor}>{gear.gearName}</color></b>\n" +
                           $"<color={rarityColor}>{gear.rarity}</color>\n\n" +
                           $"<b>Damage:</b> {gear.baseDamage}\n" +
                           $"<b>STR:</b> +{gear.strengthBonus}\n" +
                           $"<b>CON:</b> +{gear.constitutionBonus}\n" +
                           $"<b>AGI:</b> +{gear.agilityBonus}";

        AdjustTooltipPosition(position);
        tooltipPanel.SetActive(true);

        if (canvasGroup != null)
            StartCoroutine(FadeInTooltip());
    }

    public void HideTooltip()
    {
        currentGear = null;

        if (tooltipPanel == null || !tooltipPanel.activeSelf)
        {
            // If the tooltip panel is inactive, do nothing
            return;
        }

        if (canvasGroup != null)
        {
            StartCoroutine(FadeOutTooltip());
        }
        else
        {
            tooltipPanel.SetActive(false);
        }
    }

    private void AdjustTooltipPosition(Vector2 position)
    {
        if (tooltipRectTransform == null) return;

        position += tooltipOffset;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 tooltipSize = tooltipRectTransform.sizeDelta;

        if (position.x + tooltipSize.x > screenSize.x)
            position.x = screenSize.x - tooltipSize.x - 10f;

        if (position.x < 0)
            position.x = 10f;

        if (position.y - tooltipSize.y < 0)
            position.y = tooltipSize.y + 10f;

        tooltipPanel.transform.position = position;
    }

    private IEnumerator FadeInTooltip()
    {
        tooltipPanel.SetActive(true);
        canvasGroup.alpha = 0;
        float elapsedTime = 0f;
        while (elapsedTime < 0.2f)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / 0.2f);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOutTooltip()
    {
        if (canvasGroup == null || tooltipPanel == null)
        {
            yield break; // Exit if required components are missing
        }

        float elapsedTime = 0f;
        while (elapsedTime < 0.2f && tooltipPanel.activeSelf) // Check if panel is still active
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / 0.2f);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (tooltipPanel != null)
        {
            canvasGroup.alpha = 0;
            tooltipPanel.SetActive(false);
        }
    }

    private string GetRarityColor(WeaponRarity rarity)
    {
        return rarity switch
        {
            WeaponRarity.Common => "#FFFFFF",
            WeaponRarity.Uncommon => "#1EFF00",
            WeaponRarity.Rare => "#0070DD",
            WeaponRarity.Epic => "#A335EE",
            WeaponRarity.Legendary => "#FF8000",
            _ => "#FFFFFF",
        };
    }
}
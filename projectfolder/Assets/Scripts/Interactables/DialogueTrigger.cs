using UnityEngine;
using TMPro;
using System.Collections; // Add this line for IEnumerator
using UnityEngine.SceneManagement;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private string dialogueText;
    [SerializeField] private TextMeshProUGUI dialoguePopup;
    [SerializeField] private TextMeshProUGUI pressEPrompt;
    [SerializeField] private float displayDuration = 3f;

    [Header("Bloom Settings")]
    [SerializeField] private GameObject bloomVisuals;
    [SerializeField] private float fadeDuration = 1f;

    private bool isPlayerInTrigger = false;
    private bool hasInteracted = false;
    private ParticleSystem bloomParticles;

    private void Start()
    {
        if (bloomVisuals != null)
        {
            bloomParticles = bloomVisuals.GetComponent<ParticleSystem>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            isPlayerInTrigger = true;
            ShowPressEPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            HidePressEPrompt();
            HideDialogue();
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && !hasInteracted)
        {
            hasInteracted = true;
            HidePressEPrompt();
            ShowDialogue();
            StartCoroutine(FadeOutBloom());
        }
    }

    private IEnumerator FadeOutBloom()
    {
        if (bloomVisuals == null) yield break;

        float timer = 0f;

        if (fadeDuration > 0 && bloomParticles != null)
        {
            var mainModule = bloomParticles.main;
            Color initialColor = mainModule.startColor.color;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
                mainModule.startColor = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                yield return null;
            }
        }

        bloomVisuals.SetActive(false);
    }

    private void ShowPressEPrompt()
    {
        pressEPrompt?.gameObject.SetActive(true);
    }

    private void HidePressEPrompt()
    {
        pressEPrompt?.gameObject.SetActive(false);
    }

    private void ShowDialogue()
    {
        if (dialoguePopup != null)
        {
            dialoguePopup.text = dialogueText;
            dialoguePopup.gameObject.SetActive(true);
            Invoke(nameof(HideDialogue), displayDuration);

            PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
            playerInteraction?.SetDialogueActive(true);
        }
    }

    private void HideDialogue()
    {
        dialoguePopup?.gameObject.SetActive(false);

        PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
        playerInteraction?.SetDialogueActive(false);
    }
}
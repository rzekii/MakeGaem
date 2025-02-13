using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private string dialogueText; // The text to display in the dialogue popup
    [SerializeField] private TextMeshProUGUI dialoguePopup; // Reference to the UI text element for the dialogue
    [SerializeField] private TextMeshProUGUI pressEPrompt; // Reference to the "Press E" prompt UI element
    [SerializeField] private float displayDuration = 3f; // How long the dialogue should be displayed

    private bool isPlayerInTrigger = false; // Track if the player is in the trigger area

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
            isPlayerInTrigger = true; // Player is in the trigger area
            ShowPressEPrompt(); // Show the "Press E" prompt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false; // Player has left the trigger area
            HidePressEPrompt(); // Hide the "Press E" prompt
            HideDialogue(); // Hide the dialogue if the player leaves the trigger
        }
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E)) // Check if player is in trigger and presses E
        {
            HidePressEPrompt(); // Hide the "Press E" prompt
            ShowDialogue(); // Show the dialogue popup
        }
    }

    private void ShowPressEPrompt()
    {
        if (pressEPrompt != null)
        {
            pressEPrompt.text = "Press E to Interact"; // Set the "Press E" text
            pressEPrompt.gameObject.SetActive(true); // Show the "Press E" prompt
        }
    }

    private void HidePressEPrompt()
    {
        if (pressEPrompt != null)
        {
            pressEPrompt.gameObject.SetActive(false); // Hide the "Press E" prompt
        }
    }

    private void ShowDialogue()
    {
        if (dialoguePopup != null)
        {
            dialoguePopup.text = dialogueText; // Set the dialogue text
            dialoguePopup.gameObject.SetActive(true); // Show the dialogue popup
            Invoke("HideDialogue", displayDuration); // Hide the popup after the specified duration

            // Notify the PlayerInteraction script that dialogue is active
            PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
            if (playerInteraction != null)
            {
                playerInteraction.SetDialogueActive(true);
            }
        }
    }

    private void HideDialogue()
    {
        if (dialoguePopup != null)
        {
            dialoguePopup.gameObject.SetActive(false); // Hide the dialogue popup

            // Notify the PlayerInteraction script that dialogue is inactive
            PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
            if (playerInteraction != null)
            {
                playerInteraction.SetDialogueActive(false);
            }
        }
    }
}
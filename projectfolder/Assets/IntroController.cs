using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // Required for TextMeshProUGUI

public class IntroController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText; // Only one text reference needed
    [SerializeField] private float fadeSpeed = 0.5f;
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private string nextSceneName = "CityHub";

    private void Start()
    {
        if (titleText == null)
        {
            Debug.LogError("Title Text is not assigned in the Inspector!");
            return;
        }

        InitializeTitle();
        StartCoroutine(IntroSequence());
    }

    private void InitializeTitle()
    {
        Color initialColor = titleText.color;
        initialColor.a = 0f; // Fully transparent
        titleText.color = initialColor;
    }

    private IEnumerator IntroSequence()
    {
        yield return FadeInTitle();
        yield return new WaitForSeconds(displayTime);
        LoadNextScene();
    }

    private IEnumerator FadeInTitle()
    {
        while (titleText.color.a < 1f)
        {
            Color currentColor = titleText.color;
            currentColor.a += fadeSpeed * Time.deltaTime;
            titleText.color = currentColor;
            yield return null;
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not specified!");
        }
    }

    // Optional: Skip intro with any key press
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            LoadNextScene();
        }
    }
}
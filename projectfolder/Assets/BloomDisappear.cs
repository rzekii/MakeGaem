using UnityEngine;
using System.Collections; // Required for IEnumerator

public class BloomDisappear : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject bloomVisuals; // Assign particle system/light
    [SerializeField] private float fadeDuration = 1f; // Optional fade-out

    [Header("Dialogue")]
    [SerializeField] private DialogueTrigger dialogueTrigger; // Assign your existing component

    private bool _alreadyInteracted;
    private ParticleSystem _bloomParticles; // Cache the particle system

    private void Start()
    {
        // Auto-get references
        if (dialogueTrigger == null)
            dialogueTrigger = GetComponent<DialogueTrigger>();

        if (bloomVisuals != null)
            _bloomParticles = bloomVisuals.GetComponent<ParticleSystem>();
    }

    public void OnBloomInteracted()
    {
        if (_alreadyInteracted || bloomVisuals == null) return;

        _alreadyInteracted = true;
        StartCoroutine(FadeOutBloom());
    }

    private IEnumerator FadeOutBloom()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            if (_bloomParticles != null)
            {
                var main = _bloomParticles.main;
                main.startColor = new Color(1, 1, 1, 1 - (timer / fadeDuration));
            }
            timer += Time.deltaTime;
            yield return null;
        }

        bloomVisuals.SetActive(false);

        // Optional: Destroy completely after hiding
        // Destroy(bloomVisuals); 
    }
}
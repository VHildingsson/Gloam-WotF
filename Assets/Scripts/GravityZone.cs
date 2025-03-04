using UnityEngine;
using System.Collections;

public class GravityZone : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float lowGravityScale = 0.5f; // Gravity inside the zone
    public float exitGravityScale = 5f; // Default gravity when leaving
    public float floatyDrag = 2f; // Extra drag for floaty movement

    [Header("Audio Settings")]
    public float fadeDuration = 1.0f; // Duration of fade in/out
    private Coroutine fadeCoroutine;

    private AudioSource audioSource;
    [SerializeField] private AudioClip gravitySound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // Ensure looping is enabled
        audioSource.playOnAwake = false; // Avoid unwanted play
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = lowGravityScale;
                rb.drag = floatyDrag;
                Debug.Log("Entered Low Gravity Zone");

                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeAudio(0.5f)); // Fade in
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = exitGravityScale;
                rb.drag = 0f;
                Debug.Log("Exited Low Gravity Zone");

                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeAudio(0.0f)); // Fade out
            }
        }
    }

    private IEnumerator FadeAudio(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        if (targetVolume > 0 && !audioSource.isPlaying)
        {
            audioSource.clip = gravitySound;
            audioSource.Play(); // Start playing when fading in
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (targetVolume == 0)
        {
            audioSource.Stop(); // Stop when fully faded out
        }
    }
}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAudioFadeIn : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeDuration = 3.0f; // Time it takes to fade in

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0; // Start with 0 volume
        StartCoroutine(FadeInAudio());
    }

    IEnumerator FadeInAudio()
    {
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }

        audioSource.volume = 1; // Ensure it's fully up at the end
    }
}

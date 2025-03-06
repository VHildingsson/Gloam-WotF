using UnityEngine;
using System.Collections;

public class IntroCutsceneAudioTrigger : MonoBehaviour
{
    public AudioSource audioSource; // Assign in Inspector
    public float delayTime = 36.5f; // Time to wait before playing the sound

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource not assigned in IntroCutsceneAudioTrigger!");
            return;
        }

        // Start coroutine to play sound after delay
        StartCoroutine(PlaySoundAfterDelay());
    }

    private IEnumerator PlaySoundAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        audioSource.Play();
    }
}


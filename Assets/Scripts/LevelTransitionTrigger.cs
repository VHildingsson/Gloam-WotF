using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelTransitionTrigger : MonoBehaviour
{
    [Header("Transition Settings")]
    public string nextLevel = "Level2"; // The next scene to load
    public float transitionDelay = 2f; // Time before switching scenes
    public FadeScreen fadeScreen; // Reference to the fade effect

    private bool hasTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            // Disable Player Movement
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.DisableMovement(float.MaxValue); // Disable movement indefinitely
                Debug.Log("?? Player movement disabled during transition.");
            }

            StartCoroutine(TransitionToNextLevel());
        }
    }

    private IEnumerator TransitionToNextLevel()
    {
        Debug.Log("?? Level Transition Triggered...");

        // Start fade-out effect if fadeScreen is available
        if (fadeScreen != null)
        {
            fadeScreen.FadeOut();
        }
        else
        {
            Debug.LogWarning("?? FadeScreen not assigned! Scene will transition without fade.");
        }

        yield return new WaitForSeconds(transitionDelay);

        Debug.Log("?? Loading Next Level: " + nextLevel);
        SceneManager.LoadScene(nextLevel);
    }
}



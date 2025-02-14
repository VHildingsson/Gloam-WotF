using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TrueDeathCutscene : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public float waitBeforeSwitch = 4f; // Adjustable in Inspector

    [Header("Animation References")]
    public Animator childAnimator;
    public Animator bossAnimator;

    void Start()
    {
        Debug.Log("?? Entered True Death Cutscene");

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
            Debug.Log("?? Player Disabled for Cutscene");
        }

        // Play animations
        if (childAnimator != null)
        {
            childAnimator.Play("TrueDeathChild");
            Debug.Log("? Playing TrueDeathChild Animation");
        }
        else
        {
            Debug.LogWarning("? TrueDeathChild Animator is missing!");
        }

        if (bossAnimator != null)
        {
            bossAnimator.Play("TrueDeathBoss");
            Debug.Log("? Playing TrueDeathBoss Animation");
        }
        else
        {
            Debug.LogWarning("? TrueDeathBoss Animator is missing!");
        }

        StartCoroutine(PlayCutscene());
    }


    private IEnumerator PlayCutscene()
    {
        // Wait for animations to finish
        yield return new WaitForSeconds(waitBeforeSwitch);

        // Switch to the final Game Over scene
        SceneManager.LoadScene("FinalGameOverScene");
    }
}






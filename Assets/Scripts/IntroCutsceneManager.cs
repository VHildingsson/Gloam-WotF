using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroCutsceneManager : MonoBehaviour
{
    private Animator animator;
    private float cutsceneLength; // Store animation length

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No Animator found on IntroCutsceneManager!");
            return;
        }

        // Play the cutscene animation
        animator.Play("IntroCutsceneAnimation");

        // Get animation clip length dynamically
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            cutsceneLength = clipInfo[0].clip.length;
        }
        else
        {
            Debug.LogError("No animation clip found!");
            cutsceneLength = 5f; // Default failsafe
        }

        // Start coroutine to load next scene after animation
        StartCoroutine(WaitForCutsceneToEnd());
    }

    private IEnumerator WaitForCutsceneToEnd()
    {
        yield return new WaitForSeconds(cutsceneLength);

        // Load Level1 when the animation ends
        SceneManager.LoadScene("Level1");
    }
}


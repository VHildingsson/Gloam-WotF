using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroCutsceneManager : MonoBehaviour
{
    private Animator animator;
    private float cutsceneLength; // Store animation length
    private bool cutsceneSkipped = false; // Track if cutscene was skipped

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

    void Update()
    {
        // Check if 'E' key is pressed
        if (Input.GetKeyDown(KeyCode.E) && !cutsceneSkipped)
        {
            SkipCutscene();
        }
    }

    private IEnumerator WaitForCutsceneToEnd()
    {
        yield return new WaitForSeconds(cutsceneLength);
        if (!cutsceneSkipped) // Prevent double scene loading
        {
            SceneManager.LoadScene("Level1");
        }
    }

    private void SkipCutscene()
    {
        cutsceneSkipped = true;
        SceneManager.LoadScene("Level1");
    }
}



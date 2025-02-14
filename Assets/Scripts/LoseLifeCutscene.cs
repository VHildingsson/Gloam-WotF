using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoseLifeCutscene : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public float waitBeforeSwitch = 3f; // ? Adjustable in Inspector

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
            Debug.Log("?? Player Disabled for Cutscene");
        }

        StartCoroutine(PlayCutscene());
    }


    private IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(waitBeforeSwitch); // ? Now customizable

        // ? If the player still has lives left, respawn them
        if (GameManager.instance.HasLivesLeft())
        {
            SceneManager.LoadScene("RespawnScene");
        }
        else
        {
            // ? If NO lives are left, transition to the True Death scene instead
            SceneManager.LoadScene("TrueDeathCutsceneScene");
        }
    }
}







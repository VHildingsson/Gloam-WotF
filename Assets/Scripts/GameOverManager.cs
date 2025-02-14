using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.SetActive(false);
            Debug.Log("?? Player Disabled for Cutscene");
        }
    }

    private void Update()
    {
        // ? Restart game when ANY key is pressed
        if (Input.anyKeyDown)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        Debug.Log("?? Restart Button Pressed - Restarting Game...");
        GameManager.instance.RestartGame();
    }

    public void QuitGame()
    {
        Debug.Log("?? Exiting Game...");
        Application.Quit();
    }
}





using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class RespawnManager : MonoBehaviour
{
    public TMP_Text respawnText;
    public float waitBeforeSwitch = 2f;

    void Start()
    {
        Debug.Log("?? RespawnManager Start Called");

        GameManager.EnsureGameManagerExists();

        if (GameManager.instance == null)
        {
            Debug.LogError("? GameManager still missing after EnsureGameManagerExists!");
            return; // ?? Prevents further execution if GameManager is missing
        }

        // ?? 1. Find the player in the scene
        GameObject player = GameObject.FindWithTag("Player");

        // ?? 2. If a player exists, disable it for the RespawnScene
        if (player != null)
        {
            Debug.Log("?? Disabling Player in Respawn Scene...");
            player.SetActive(false);
        }
        else
        {
            Debug.LogWarning("?? No Player found! Attempting to Spawn...");
            player = GameManager.instance.SpawnPlayer();

            if (player != null)
            {
                Debug.Log("? Player Spawned: " + player.name);
                player.SetActive(false); // ?? Keep player disabled during RespawnScene
                Debug.Log("?? Spawned Player Disabled in Respawn Scene");
            }
            else
            {
                Debug.LogError("? Player is STILL missing after SpawnPlayer() call!");
            }
        }

        StartCoroutine(WaitForRespawn());
    }





    private IEnumerator WaitForRespawn()
    {
        if (respawnText != null)
        {
            respawnText.gameObject.SetActive(true);
        }

        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        if (respawnText != null)
        {
            respawnText.gameObject.SetActive(false);
        }

        if (GameManager.instance == null)
        {
            Debug.LogError("? GameManager is missing! Cannot get respawn position.");
            yield break;
        }

        string lastLevel = GameManager.instance.GetLastLevel();
        Debug.Log("?? Loading Last Level: " + lastLevel);
        if (string.IsNullOrEmpty(lastLevel))
        {
            Debug.LogError("? Last level name is missing! Loading default level.");
            lastLevel = "Level1";
        }

        SceneManager.LoadScene(lastLevel);

        yield return new WaitForSeconds(0.1f);

        // ? Ensure the Player is Spawned
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.Log("? No Player found! Spawning new player.");
            player = GameManager.instance.SpawnPlayer();
        }

        if (player != null)
        {
            player.transform.position = GameManager.instance.GetRespawnPosition();
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.SetActive(true); // ?? Ensure player is reactivated
            Debug.Log("? Moving Player to: " + GameManager.instance.GetRespawnPosition());

            GameManager.instance.UpdatePlayerReferences(player);
        }
    }
}











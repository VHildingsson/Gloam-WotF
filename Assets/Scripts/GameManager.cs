using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool hasGameRestarted = false;

    [Header("Lives System")]
    public int maxLives = 3;
    private int currentLives;
    private bool hasUsedFinalLife = false;

    [Header("Respawn System")]
    public string playerPrefabPath = "Player"; // ? Player prefab name in Resources
    public string gameManagerPrefabPath = "GameManager"; // ? GameManager prefab path in Resources
    private GameObject playerPrefab;
    private Vector3 lastRespawnPosition;
    private string lastLevel;
    private Transform worldSpawn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Debug.Log("?? GameManager already exists, destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        currentLives = maxLives;

        // ? Load Player Prefab from Resources
        if (playerPrefab == null)
        {
            playerPrefab = Resources.Load<GameObject>("Player");
            if (playerPrefab == null)
            {
                Debug.LogError("? Player Prefab not found in Resources!");
            }
        }

        FindWorldSpawn();
        EnsurePlayerExists();

    }

    public bool HasGameRestartedRecently()
    {
        return hasGameRestarted;
    }

    private void FindWorldSpawn()
    {
        GameObject spawnObject = GameObject.FindGameObjectWithTag("WorldSpawn");

        if (spawnObject != null)
        {
            worldSpawn = spawnObject.transform;
            lastRespawnPosition = worldSpawn.position;
            Debug.Log("?? Found WorldSpawn at: " + lastRespawnPosition);
        }
        else
        {
            Debug.LogError("? WorldSpawn not found in scene! Defaulting to (0,0)");
            lastRespawnPosition = Vector3.zero;
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("?? Scene Loaded: " + scene.name);

        // ? Only update `lastLevel` if we're in an actual game level (Level1, Level2, etc.)
        if (scene.name.StartsWith("Level"))
        {
            lastLevel = scene.name;
            Debug.Log("? Last Level Updated: " + lastLevel);
        }

        // ? Ensure the player exists after scene loads
        EnsureGameManagerExists();
        EnsurePlayerExists();

        // ? Reassign player references AFTER scene finishes loading
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = GetRespawnPosition();
            UpdatePlayerReferences(player);
        }
        else
        {
            Debug.LogError("? Player still missing after scene load!");
        }
    }




    private void EnsurePlayerExists()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.Log("?? No Player found in scene! Spawning a new one...");
            SpawnPlayer();
        }
        else
        {
            Debug.Log("? Player already exists in scene.");
        }

    }


    public static void EnsureGameManagerExists()
    {
        if (instance == null)
        {
            Debug.Log("?? Creating new GameManager instance.");
            GameObject gmPrefab = Resources.Load<GameObject>("GameManager");
            if (gmPrefab != null)
            {
                GameObject newGM = Instantiate(gmPrefab);
                newGM.name = "GameManager"; // ? Ensure it's named properly
                DontDestroyOnLoad(newGM);
                Debug.Log("? GameManager Created");
            }
            else
            {
                Debug.LogError("? GameManager Prefab not found in Resources!");
            }
        }
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }


    public void SetRespawnPoint(Vector3 newRespawnPosition)
    {
        if (newRespawnPosition == Vector3.zero)
        {
            Debug.LogError("? Attempted to set respawn point to (0,0,0) - Skipping.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        // ? Ensure respawn is set only in actual game levels (e.g., "Level1", "Level2")
        if (!currentScene.StartsWith("Level"))
        {
            Debug.LogWarning("?? Ignoring respawn point update in non-level scene: " + currentScene);
            return;
        }

        // ? Ensure the first checkpoint works after a restart
        if (hasGameRestarted)
        {
            Debug.Log("?? Resetting Respawn Point to WorldSpawn after full restart.");
            lastRespawnPosition = worldSpawn.position;
            lastLevel = "Level1";
            hasGameRestarted = false; // ? Allow checkpoints to work again
            return;
        }

        // ? Store the new valid respawn point
        lastRespawnPosition = newRespawnPosition;
        lastLevel = currentScene; // ? Ensures only real levels update lastLevel

        Debug.Log("?? Respawn Point Set: " + lastRespawnPosition + " in Level: " + lastLevel);
    }

    public void SetRespawnPointToWorldSpawn()
    {
        if (worldSpawn != null)
        {
            lastRespawnPosition = worldSpawn.position;
            Debug.Log("?? Respawn point set to WorldSpawn: " + lastRespawnPosition);
        }
        else
        {
            Debug.LogError("? WorldSpawn is missing! Defaulting respawn to (0,0,0)");
            lastRespawnPosition = Vector3.zero;
        }
    }



    public Vector3 GetRespawnPosition()
    {
        // ? If we have a valid checkpoint, use it
        if (lastRespawnPosition != Vector3.zero)
        {
            Debug.Log("? Returning Respawn Position: " + lastRespawnPosition);
            return lastRespawnPosition;
        }

        // ? If no checkpoint, return WorldSpawn
        if (worldSpawn != null)
        {
            Debug.LogWarning("?? No checkpoint found! Using WorldSpawn: " + worldSpawn.position);
            return worldSpawn.position;
        }

        // ? If WorldSpawn is missing, fall back to (0,0)
        Debug.LogError("? WorldSpawn is NULL! Defaulting to (0,0,0)");
        return Vector3.zero;
    }




    public string GetLastLevel()
    {
        // ? If lastLevel is empty, check if we're currently in a valid game level
        if (string.IsNullOrEmpty(lastLevel) || !lastLevel.StartsWith("Level"))
        {
            string currentScene = SceneManager.GetActiveScene().name;

            // ? Only use the current scene if it's a proper game level
            if (currentScene.StartsWith("Level"))
            {
                lastLevel = currentScene;
                Debug.LogWarning("?? Last level was NULL! Setting to current level: " + lastLevel);
            }
            else
            {
                lastLevel = "Level1"; // Default to Level1 if no valid last level exists
                Debug.LogError("? Last level was invalid! Defaulting to Level1.");
            }
        }

        return lastLevel;
    }



    public bool HasLivesLeft()
    {
        return currentLives >= 0;
    }

    public void LoseLife()
    {
        if (currentLives >= 1)
        {
            currentLives--;
            SceneManager.LoadScene("LoseLifeCutsceneScene");
        }
        else if (currentLives == 1 && !hasUsedFinalLife)
        {
            currentLives--;
            hasUsedFinalLife = true;
            SceneManager.LoadScene("LoseLifeCutsceneScene");
        }
        else
        {
            TrueDeath();
        }
    }

    public void TrueDeath()
    {
        SceneManager.LoadScene("TrueDeathCutsceneScene");
    }

    public void RestartGame()
    {
        Debug.Log("?? Restarting Game...");

        currentLives = maxLives;
        hasUsedFinalLife = false;

        // ? Reset respawn position ONLY to WorldSpawn
        lastRespawnPosition = Vector3.zero;
        lastLevel = "Level1";

        if (worldSpawn == null)
        {
            Debug.LogWarning("?? WorldSpawn was NULL! Finding it again.");
            FindWorldSpawn();
        }

        if (worldSpawn != null)
        {
            lastRespawnPosition = worldSpawn.position;
            Debug.Log("?? Respawn Reset to WorldSpawn: " + lastRespawnPosition);
        }
        else
        {
            Debug.LogError("? WorldSpawn STILL NULL! Defaulting to (0,0,0)");
            lastRespawnPosition = Vector3.zero;
        }

        // ? Clear checkpoint data
        PlayerPrefs.DeleteKey("LastRespawnPosition");
        PlayerPrefs.DeleteKey("LastLevel");
        PlayerPrefs.Save();

        // ? Reload Level1
        Debug.Log("?? Reloading Level1...");
        SceneManager.LoadScene("Level1");
    }



    public void UpdatePlayerReferences(GameObject newPlayer)
    {
        StartCoroutine(DelayedReferenceUpdate(newPlayer)); // ? Run after a delay
    }

    private IEnumerator DelayedReferenceUpdate(GameObject newPlayer)
    {
        yield return new WaitForSeconds(0.1f); // ? Small delay to ensure scene loads

        Debug.Log("?? Updating Player References for all scripts...");

        // ?? Assign Player to CameraFollow
        CameraFollow camera = FindObjectOfType<CameraFollow>();
        if (camera != null)
        {
            camera.target = newPlayer.transform;
            Debug.Log("? CameraFollow Target Updated");
        }

        // ?? Assign Player to SeedOfSight
        SeedOfSight[] seeds = FindObjectsOfType<SeedOfSight>(true); // `true` finds inactive objects too!
        if (seeds.Length > 0)
        {
            foreach (SeedOfSight seed in seeds)
            {
                seed.player = newPlayer.transform;
            }
            Debug.Log($"? {seeds.Length} SeedOfSight objects updated with player reference.");
        }
        else
        {
            Debug.LogWarning("?? No SeedOfSight objects found in the scene!");
        }

        // ?? Assign Player to AltarOfLight
        AltarOfLight altar = FindObjectOfType<AltarOfLight>();
        if (altar != null)
        {
            altar.player = newPlayer.transform;
            Debug.Log("? AltarOfLight Player Updated");
        }
    }

    public GameObject SpawnPlayer()
    {
        Debug.Log("?? SpawnPlayer() Called");

        // ?? 1. Destroy any existing player
        GameObject oldPlayer = GameObject.FindWithTag("Player");
        if (oldPlayer != null)
        {
            Debug.Log("?? Destroying old player before respawn.");
            Destroy(oldPlayer);
        }

        // ?? 2. Check if Player Prefab Exists
        if (playerPrefab == null)
        {
            Debug.LogError("? Player prefab is missing! Ensure it's inside Resources.");
            return null;
        }

        // ?? 3. Spawn the new player
        Vector3 spawnPosition = GetRespawnPosition();
        spawnPosition.z = 0;
        GameObject newPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        newPlayer.name = "Player";

        // ?? 4. Ensure new player is ACTIVE
        newPlayer.SetActive(true);

        // ?? 5. Reassign Player to all relevant scripts
        UpdatePlayerReferences(newPlayer);

        DontDestroyOnLoad(newPlayer); // ? Keeps player when switching scenes

        Debug.Log("? Player Spawned at: " + spawnPosition);
        return newPlayer;
    }




}









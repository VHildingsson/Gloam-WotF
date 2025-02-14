using UnityEngine;

public class LostChrysalid : MonoBehaviour
{
    private bool isActivated = false;
    private Animator anim;
    public Transform respawnPoint; // ? Assign in Inspector as a child object of Chrysalid

    void Start()
    {
        anim = GetComponent<Animator>();

        if (respawnPoint == null)
        {
            Debug.LogError("? Respawn point is NOT assigned to LostChrysalid: " + gameObject.name);
        }

        // ? Ensure GameManager exists before interacting with it
        GameManager.EnsureGameManagerExists();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated && other.CompareTag("Player"))
        {
            ActivateChrysalid(other.gameObject);
        }
    }

    void ActivateChrysalid(GameObject player)
    {
        if (GameManager.instance == null) return;

        if (isActivated)
        {
            Debug.Log("? Checkpoint already activated, skipping.");
            return;
        }

        isActivated = true;
        anim.SetTrigger("Bloom");

        if (respawnPoint == null)
        {
            Debug.LogError("? Respawn Point is NULL! Cannot set checkpoint.");
            return;
        }

        if (!respawnPoint.gameObject.activeInHierarchy)
        {
            Debug.Log("? Reactivating Respawn Point...");
            respawnPoint.gameObject.SetActive(true);
        }

        // ? Always allow checkpoint activation
        GameManager.instance.SetRespawnPoint(respawnPoint.position);
        Debug.Log("? Respawn Point Set at " + respawnPoint.position);
    }


}







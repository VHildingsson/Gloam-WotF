using UnityEngine;
using System.Collections;

public class SeedOfSight : MonoBehaviour
{
    public bool isHeld = false;
    public Transform player;
    public Transform altar;
    public float pickupRange = 2f;
    public float useRange = 2f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode useKey = KeyCode.F;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isPlaced = false;

    private Vector3 originalPosition; // ?? Store original spawn position

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // ?? Store the seed's original position
        originalPosition = transform.position;

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
                Debug.Log("? SeedOfSight - Player Assigned: " + player.position);
            }
            else
            {
                Debug.LogError("? SeedOfSight - No Player Found!");
            }
        }
    }

    void Update()
    {
        if (isPlaced) return; // Completely stop interactions after placement

        if (!isHeld && Vector2.Distance(transform.position, player.position) <= pickupRange && Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
        else if (isHeld && Input.GetKeyDown(pickupKey))
        {
            Drop();
        }

        if (isHeld && Vector2.Distance(transform.position, altar.position) <= useRange && Input.GetKeyDown(useKey))
        {
            UseAtAltar();
        }
    }

    void PickUp()
    {
        if (isPlaced) return;

        isHeld = true;
        transform.SetParent(player);
        transform.localPosition = Vector3.zero;
        rb.isKinematic = true;
        col.enabled = false;
    }

    void Drop()
    {
        if (isPlaced) return;

        isHeld = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
    }

    void UseAtAltar()
    {
        if (isPlaced) return;

        isPlaced = true;
        isHeld = false;

        Debug.Log("? Seed of Sight used at the altar!");

        transform.SetParent(null);
        transform.position = altar.GetComponent<AltarOfLight>().seedPlacementPoint.position;
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        col.enabled = false;

        this.enabled = false;

        AltarOfLight altarScript = altar.GetComponent<AltarOfLight>();
        if (altarScript != null)
        {
            altarScript.StartAltarActivation();
        }
    }

    // ?? Handle Respawning when Destroyed
    public void RespawnSeed()
    {
        Debug.Log("?? Seed Destroyed! Respawning at original position...");

        // Reset seed to its original position
        transform.position = originalPosition;

        // Reset all properties
        isHeld = false;
        isPlaced = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.None;
        col.enabled = true;
        this.enabled = true;

        // ?? Reassign the player reference after respawn
        GameObject foundPlayer = GameObject.FindWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
            Debug.Log("? Player Reassigned to Seed after Respawn.");
        }
        else
        {
            Debug.LogError("? Player NOT found after Seed Respawn!");
        }
    }

    // ?? Collision Handling (For Spikes or Death Triggers)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("KillZone")) // Tag spikes or bottomless pits as "KillZone"
        {
            Debug.Log("?? Seed hit a deadly area!");
            StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(1f); // Small delay before respawning
        RespawnSeed();
    }
}










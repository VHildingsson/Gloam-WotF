using UnityEngine;
using System.Collections;

public class AltarOfLight : MonoBehaviour
{
    public bool isActivated = false;
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.F;
    public Transform player;
    public Animator animator;
    public Transform seedPlacementPoint;
    private SeedOfSight placedSeed;
    public bool canInteract = true;
    public RockCollapse fallingRock;

    [Header("Linked Rock Collapse")]
    public float delayBeforeCollapse = 1.5f; // Time before the rock falls

    void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
                Debug.Log("? AltarOfLight - Player Assigned: " + player.position);
            }
            else
            {
                Debug.LogError("? AltarOfLight - No Player Found!");
            }
        }
    }

    void Update()
    {
        if (canInteract && Vector2.Distance(transform.position, player.position) <= interactionRange && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    void Interact()
    {
        if (placedSeed == null)
        {
            SeedOfSight seed = player.GetComponentInChildren<SeedOfSight>();
            if (seed != null)
            {
                PlaceSeed(seed);
            }
            else
            {
                Debug.Log("?? The altar is inactive. You need the Seed of Sight to activate it.");
            }
        }
        else
        {
            Debug.Log("? The Seed of Sight is already placed and active.");
        }
    }

    void PlaceSeed(SeedOfSight seed)
    {
        placedSeed = seed;

        // Ensure the seed is fully centered on the altar
        seed.transform.SetParent(null);
        seed.transform.position = seedPlacementPoint.position;
        seed.transform.rotation = Quaternion.identity;
        seed.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        Rigidbody2D rb = seed.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // Lock the seed in place
        }

        // Disable seed interactions
        seed.GetComponent<Collider2D>().enabled = false;
        seed.enabled = false;

        StartAltarActivation();
    }

    public void StartAltarActivation()
    {
        canInteract = false; // Disable altar interaction immediately

        animator.SetTrigger("AltarMidActive"); // Play altar animation

        if (fallingRock != null)
        {
            StartCoroutine(TriggerRockFallDelayed()); // Delay rock fall for timing
        }

        StartCoroutine(CompleteAltarActivation());
    }

    private IEnumerator TriggerRockFallDelayed()
    {
        yield return new WaitForSeconds(delayBeforeCollapse);

        if (fallingRock != null)
        {
            fallingRock.TriggerRockFall();
        }
        else
        {
            Debug.LogWarning("?? FallingRock is not assigned!");
        }
    }


    private IEnumerator CompleteAltarActivation()
    {
        yield return new WaitForSeconds(1.0f);
        animator.SetTrigger("AltarActive");
        isActivated = true;
        Debug.Log("? The Altar of Light has been fully activated!");
    }
}












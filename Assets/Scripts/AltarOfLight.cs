using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Needed to check level name

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

    [Header("Special Key Activation for Level 2")]
    public GameObject keyToEnable; // Assign the key object in the inspector

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
                Debug.LogError("?? AltarOfLight - No Player Found!");
            }
        }

        // Ensure key is disabled at start (only if we're in Level 2)
        if (SceneManager.GetActiveScene().name == "Level2" && keyToEnable != null)
        {
            keyToEnable.SetActive(false);
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
                Debug.Log("?? The altar is inactive. You need the Seed of Sight.");
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

        seed.transform.SetParent(null);
        seed.transform.position = seedPlacementPoint.position;
        seed.transform.rotation = Quaternion.identity;
        seed.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        Rigidbody2D rb = seed.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        seed.GetComponent<Collider2D>().enabled = false;
        seed.enabled = false;

        StartAltarActivation();
    }

    public void StartAltarActivation()
    {
        canInteract = false;
        animator.SetTrigger("AltarMidActive");

        if (fallingRock != null)
        {
            StartCoroutine(TriggerRockFallDelayed());
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
            Debug.LogWarning("? FallingRock is not assigned!");
        }
    }

    private IEnumerator CompleteAltarActivation()
    {
        yield return new WaitForSeconds(1.0f);
        animator.SetTrigger("AltarActive");
        isActivated = true;
        Debug.Log("? The Altar of Light has been fully activated!");

        // **Enable Key Only in Level 2**
        if (SceneManager.GetActiveScene().name == "Level2" && keyToEnable != null)
        {
            keyToEnable.SetActive(true);
            Debug.Log("?? Key has been enabled in Level 2!");
        }
    }
}













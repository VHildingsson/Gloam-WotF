using UnityEngine;
using System.Collections;

public class LockedDoor : MonoBehaviour
{
    public bool isUnlocked = false;
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.F;
    private Transform player;
    public Animator animator;
    public Transform keyPlacementPoint;
    private KeyItem placedKey;
    public bool canInteract = true;

    void Start()
    {
        StartCoroutine(AssignPlayer());
    }

    private IEnumerator AssignPlayer()
    {
        while (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;

            yield return null;
        }
    }

    void Update()
    {
        if (player == null || !canInteract) return;

        if (Vector2.Distance(transform.position, player.position) <= interactionRange && Input.GetKeyDown(interactKey))
        {
            TryUnlockDoor();
        }
    }

    void TryUnlockDoor()
    {
        if (placedKey == null)
        {
            KeyItem key = player.GetComponentInChildren<KeyItem>();

            if (key != null)
            {
                PlaceKey(key);
            }
            else
            {
                Debug.Log("?? The door is locked. You need a key.");
            }
        }
        else
        {
            Debug.Log("? The door is already unlocked.");
        }
    }

    void PlaceKey(KeyItem key)
    {
        placedKey = key;

        key.transform.SetParent(transform, true); // ?? Keep world position/scale
        key.transform.position = keyPlacementPoint.position;
        key.transform.rotation = Quaternion.identity;

        // ? Store the key's original scale and reapply it
        Vector3 originalScale = key.transform.localScale;

        Rigidbody2D rb = key.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        key.GetComponent<Collider2D>().enabled = false;
        key.enabled = false;

        // ?? Reapply original scale to prevent unwanted resizing
        key.transform.localScale = originalScale;

        StartDoorUnlock();
    }

    public void StartDoorUnlock()
    {
        canInteract = false;
        animator.SetTrigger("OpenDoor");
        StartCoroutine(CompleteDoorUnlock());
    }

    private IEnumerator CompleteDoorUnlock()
    {
        yield return new WaitForSeconds(1.0f);
        isUnlocked = true;
        Debug.Log("?? The door is now open, and the key moves with it!");
    }
}











using UnityEngine;
using System.Collections;

public class LockedDoor : MonoBehaviour
{
    public bool isUnlocked = false;
    public float interactionRange = 2f;
    public KeyCode interactKey = KeyCode.F;
    public Transform player;
    public Animator animator;
    public Transform keyPlacementPoint;
    private KeyItem placedKey;
    public bool canInteract = true;

    void Start()
    {
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        while (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
            yield return null;
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
        if (placedKey == null)
        {
            KeyItem key = player.GetComponentInChildren<KeyItem>();

            if (key != null)
            {
                PlaceKey(key);
            }
        }
    }

    void PlaceKey(KeyItem key)
    {
        placedKey = key;

        key.transform.SetParent(null);
        key.transform.position = keyPlacementPoint.position;
        key.transform.rotation = Quaternion.identity;
        key.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        Rigidbody2D rb = key.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        key.GetComponent<Collider2D>().enabled = false;
        key.enabled = false;

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
        gameObject.SetActive(false);
    }
}











using UnityEngine;
using System.Collections;

public class KeyItem : MonoBehaviour
{
    public bool isHeld = false;
    private Transform player;
    private Collider2D col;
    private Rigidbody2D rb;

    public float pickupRange = 2f;
    public float useRange = 2f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode useKey = KeyCode.F;

    private bool isPlaced = false;
    private Transform door;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // ? Run FindPlayerAndDoor EVERY time the key gets enabled
    private void OnEnable()
    {
        StartCoroutine(FindPlayerAndDoor());
    }

    private IEnumerator FindPlayerAndDoor()
    {
        // Keep checking until the player is found
        while (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;

            yield return null;
        }

        // Keep checking until the door is found
        while (door == null)
        {
            GameObject foundDoor = GameObject.FindWithTag("LockedDoor");
            if (foundDoor != null)
                door = foundDoor.transform;

            yield return null;
        }
    }

    void Update()
    {
        if (isPlaced || player == null || door == null) return; // Prevent interaction after placing the key

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceToDoor = Vector2.Distance(transform.position, door.position);

        if (!isHeld && distanceToPlayer <= pickupRange && Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
        else if (isHeld && Input.GetKeyDown(pickupKey))
        {
            Drop();
        }

        if (isHeld && distanceToDoor <= useRange && Input.GetKeyDown(useKey))
        {
            UseOnDoor();
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
        Debug.Log("? Picked up the Key!");
    }

    void Drop()
    {
        if (isPlaced) return;

        isHeld = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
        Debug.Log("?? Dropped the Key!");
    }

    void UseOnDoor()
    {
        if (isPlaced) return;
        if (door == null)
        {
            Debug.LogError("?? No door found! Make sure the door is tagged as 'LockedDoor'.");
            return;
        }

        isPlaced = true;
        isHeld = false;

        Debug.Log("?? Key used on the door!");

        transform.SetParent(door);
        transform.position = door.GetComponent<LockedDoor>().keyPlacementPoint.position;
        transform.rotation = Quaternion.identity;

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        col.enabled = false;

        gameObject.tag = "UsedKey";
        this.enabled = false;

        LockedDoor doorScript = door.GetComponent<LockedDoor>();
        if (doorScript != null)
        {
            doorScript.StartDoorUnlock();
        }
    }
}









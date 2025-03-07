using UnityEngine;
using System.Collections;

public class KeyItem : MonoBehaviour
{
    public bool isHeld = false;
    public Transform player;
    public Transform door;
    public float pickupRange = 2f;
    public float useRange = 2f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode useKey = KeyCode.F;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isPlaced = false;

    private Vector3 originalPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        originalPosition = transform.position;

        StartCoroutine(FindReferences());
    }

    private IEnumerator FindReferences()
    {
        while (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
            yield return null;
        }

        while (door == null)
        {
            GameObject foundDoor = GameObject.FindWithTag("LockedDoor");
            if (foundDoor != null) door = foundDoor.transform;
            yield return null;
        }
    }

    void Update()
    {
        if (isPlaced) return;

        if (!isHeld && Vector2.Distance(transform.position, player.position) <= pickupRange && Input.GetKeyDown(pickupKey))
        {
            PickUp();
        }
        else if (isHeld && Input.GetKeyDown(pickupKey))
        {
            Drop();
        }

        if (isHeld && door != null && Vector2.Distance(transform.position, door.position) <= useRange && Input.GetKeyDown(useKey))
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
    }

    void Drop()
    {
        if (isPlaced) return;

        isHeld = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        col.enabled = true;
    }

    void UseOnDoor()
    {
        if (isPlaced) return;

        isPlaced = true;
        isHeld = false;

        transform.SetParent(null);
        transform.position = door.GetComponent<LockedDoor>().keyPlacementPoint.position;
        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        col.enabled = false;
        this.enabled = false;

        LockedDoor doorScript = door.GetComponent<LockedDoor>();
        if (doorScript != null)
        {
            doorScript.StartDoorUnlock();
        }
    }
}







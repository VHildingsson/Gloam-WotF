using UnityEngine;

public class GravityZone : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float lowGravityScale = 0.5f; // Gravity inside the zone
    public float exitGravityScale = 5f; // Default gravity when leaving
    public float floatyDrag = 2f; // Extra drag for floaty movement

    private static int gravityZoneCount = 0; // Tracks number of active gravity zones

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            if (rb != null)
            {
                gravityZoneCount++;
                rb.gravityScale = lowGravityScale;
                rb.drag = floatyDrag;

                // Notify PlayerMovement to handle audio
                if (playerMovement != null)
                {
                    playerMovement.HandleGravityAudio(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            if (rb != null)
            {
                gravityZoneCount--;

                if (gravityZoneCount <= 0)
                {
                    rb.gravityScale = exitGravityScale;
                    rb.drag = 0f;

                    // Notify PlayerMovement to stop audio
                    if (playerMovement != null)
                    {
                        playerMovement.HandleGravityAudio(false);
                    }
                }
            }
        }
    }
}










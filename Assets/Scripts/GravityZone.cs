using UnityEngine;

public class GravityZone : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float lowGravityScale = 0.5f; // Gravity inside the zone
    public float exitGravityScale = 5f; // Default gravity when leaving
    public float floatyDrag = 2f; // Extra drag for floaty movement

    private void OnTriggerEnter2D(Collider2D other) // Use Collider2D for 2D physics
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = lowGravityScale;
                rb.drag = floatyDrag; // Makes movement feel slow and floaty
                Debug.Log("Entered Low Gravity Zone");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) // Use Collider2D for 2D physics
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = exitGravityScale;
                rb.drag = 0f; // Reset drag
                Debug.Log("Exited Low Gravity Zone");
            }
        }
    }
}



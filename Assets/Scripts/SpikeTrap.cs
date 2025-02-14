using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1; // ?? Damage spikes deal (editable in Inspector)
    public bool instantKill = false; // ?? If true, instantly kills the player

    [Header("Knockback Settings")]
    public float knockbackForce = 5f; // ?? Force applied to push the player away
    public Vector2 knockbackDirection = new Vector2(0, 1); // ? Direction of knockback (default is upwards)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                if (instantKill)
                {
                    playerHealth.TakeDamage(playerHealth.maxHealth, knockbackDirection);
                }
                else
                {
                    playerHealth.TakeDamage(damageAmount, knockbackDirection);
                }
            }
        }
    }
}


using UnityEngine;

public class StompDamage : MonoBehaviour
{
    public float bounceForce = 7f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name + " with tag: " + other.gameObject.tag);

        // ? We now check if the Player is the one entering the WeakSpot
        if (other.CompareTag("Player"))
        {
            Debug.Log("? Player has entered the WeakSpot!");

            HusklingAI enemyAI = GetComponentInParent<HusklingAI>();

            if (enemyAI != null)
            {
                Debug.Log("WeakSpot detected! Enemy dazed: " + enemyAI.isDazed);

                if (enemyAI.isDazed)
                {
                    Debug.Log("? Enemy is dazed. Calling Die() now!");
                    enemyAI.Die(); // ? Destroy the enemy

                    // ? Apply bounce to the Player
                    Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);
                    }
                }
                else
                {
                    Debug.Log("? Enemy is NOT dazed. No kill.");
                }
            }
            else
            {
                Debug.LogError("? No HusklingAI found on WeakSpot's parent!");
            }
        }
    }
}











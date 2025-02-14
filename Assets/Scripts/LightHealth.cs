using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightHealth : MonoBehaviour
{
    private Light2D light2D;
    public float maxLightIntensity = 2f;  // Max light size
    public float minLightIntensity = 0.1f; // When it reaches this, the entity dies
    private float currentLightIntensity;

    public float lightDecayRate = 0.2f; // How much light is lost per hit
    public float lightRegenRate = 0.05f; // Optional: light regained over time
    public float flickerAmount = 0.1f; // Optional: more flickering when light is low

    private bool isDead = false;

    void Start()
    {
        light2D = GetComponentInChildren<Light2D>(); // ? Finds Light2D on child object

        if (light2D == null)
        {
            Debug.LogError("No Light2D found on " + gameObject.name + " or its children!");
            return;
        }

        currentLightIntensity = maxLightIntensity;
        light2D.intensity = currentLightIntensity;
    }

    void Update()
    {
        if (!isDead)
        {
            // Gradually shrink light over time (optional)
            if (lightRegenRate > 0 && currentLightIntensity < maxLightIntensity)
            {
                ChangeLight(lightRegenRate * Time.deltaTime);
            }

            // Increase flickering when light is low
            if (light2D.intensity < maxLightIntensity * 0.3f)
            {
                light2D.intensity = Mathf.Lerp(light2D.intensity, currentLightIntensity + Random.Range(-flickerAmount, flickerAmount), Time.deltaTime * 10);
            }
        }
    }

    public void TakeDamage(float damage, Vector2 knockbackForce, GameObject source)
    {
        if (!isDead)
        {
            ChangeLight(-damage);
            Debug.Log(gameObject.name + " took damage, current light: " + currentLightIntensity);

            if (currentLightIntensity <= minLightIntensity)
            {
                Die();
            }
            else
            {
                if (gameObject.CompareTag("Player"))
                {
                    StartCoroutine(ApplyKnockback(knockbackForce, source));
                }
            }
        }
    }

    IEnumerator ApplyKnockback(Vector2 force, GameObject enemy)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero; // ? Reset movement
            rb.AddForce(force, ForceMode2D.Impulse); // ? Apply knockback

            Debug.Log("Knockback applied: " + force);

            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.DisableMovement(0.5f);
            }

            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>(), true);

            yield return new WaitForSeconds(0.5f); // ? Knockback duration

            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>(), false);
        }
    }

    private void ChangeLight(float amount)
    {
        currentLightIntensity = Mathf.Clamp(currentLightIntensity + amount, minLightIntensity, maxLightIntensity);
        light2D.intensity = currentLightIntensity;
    }

    private void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " has died!");

        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has died!");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}



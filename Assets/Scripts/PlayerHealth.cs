using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public float deathDelay = 3f;

    [Header("Knockback Settings")]
    public float knockbackForceX = 5f;
    public float knockbackForceY = 3f;
    public float knockbackDuration = 0.5f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1.5f;

    private bool isInvincible = false;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isDead = false;
    private PlayerMovement playerMovement;

    [Header("UI References")]
    [SerializeField] private FadeScreen fadeScreen;
    public float fadeOutDuration = 1f;

    private AudioSource audioSource;
    [SerializeField] private AudioClip hurtSound;



    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
        isDead = false;
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        Debug.Log("? Player took damage! Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            audioSource.PlayOneShot(hurtSound, 0.2f);
        }
        else
        {
            StartCoroutine(ApplyKnockback(knockbackDirection));
            audioSource.PlayOneShot(hurtSound, 0.2f);
        }
    }

    private IEnumerator ApplyKnockback(Vector2 knockbackDirection)
    {
        isInvincible = true;
        playerMovement.DisableMovement(knockbackDuration);
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(knockbackDirection.x * knockbackForceX, knockbackForceY), ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(invincibilityDuration - knockbackDuration);
        isInvincible = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("? Player has died!");

        rb.velocity = Vector2.zero;
        anim.SetTrigger("Die");
        playerMovement.DisableMovement(float.MaxValue);

        StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(deathDelay - fadeOutDuration);

        if (fadeScreen != null)
        {
            fadeScreen.FadeOut();
        }

        yield return new WaitForSeconds(fadeOutDuration);

        // ? Ask GameManager to handle life loss
        GameManager.instance.LoseLife();
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        anim.SetTrigger("Respawn");
        playerMovement.EnableMovement();
    }
}




















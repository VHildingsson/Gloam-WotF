using UnityEngine;
using System.Collections;

public class HusklingAI : MonoBehaviour
{
    public float speed = 7f;
    public float flipOffset = 0.5f;
    public Transform player;
    public float detectionRange = 5f;
    public float dazedTime = 3f;
    public float enemyKnockbackForceX = 5f;
    public float enemyKnockbackForceY = 2f;
    public float knockbackDuration = 0.5f;
    public float deathDelay = 2.0f;

    public LayerMask playerLayer;
    public bool startFacingRight = true; // Set initial facing direction in the Inspector

    private Animator animator;
    private Rigidbody2D rb;
    private bool canSeePlayer = false;
    public bool isDazed = false;
    private bool isRunning = false;
    private bool facingRight;
    private bool canMove = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
        rb.freezeRotation = true;

        // Set the initial facing direction based on the Inspector setting
        facingRight = startFacingRight;
        if (!facingRight)
        {
            FlipDirection(true);
        }
    }

    void Update()
    {
        if (!isDazed && canMove)
        {
            if (isRunning)
            {
                float moveDirection = facingRight ? 1 : -1;
                float currentSpeed = isDazed ? speed / 2 : speed;
                rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        animator.SetBool("canSeePlayer", canSeePlayer);
        animator.SetBool("isRunning", isRunning);
    }

    public void SetPlayerInVision(bool state)
    {
        canSeePlayer = state;

        if (canSeePlayer && canMove)
        {
            isRunning = true;
        }

        animator.SetBool("canSeePlayer", canSeePlayer);
        animator.SetBool("isRunning", isRunning);
    }

    public void OnWallHit()
    {
        if (!isDazed)
        {
            isRunning = false;
            rb.velocity = Vector2.zero;
            animator.SetTrigger("BreakMask");
            StartCoroutine(BreakMask());
        }
    }

    IEnumerator BreakMask()
    {
        SetDazed(true);
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.SetTrigger("BreakMask");

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("isDazed", true);
        yield return new WaitForSeconds(dazedTime);

        animator.SetBool("isDazed", false);
        SetDazed(false);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        FlipDirection(false);

        if (canSeePlayer)
        {
            isRunning = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isDazed)
            {
                return;
            }

            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(1, knockbackDirection);

            if (!isDazed)
            {
                ApplyEnemyKnockback(knockbackDirection);
            }
        }
    }

    private void ApplyEnemyKnockback(Vector2 direction)
    {
        if (rb != null)
        {
            canMove = false; // ? Stop normal movement temporarily

            rb.velocity = Vector2.zero; // ? Stop any existing velocity
            Vector2 finalKnockback = new Vector2(direction.x * enemyKnockbackForceX, enemyKnockbackForceY);
            rb.AddForce(finalKnockback * rb.mass, ForceMode2D.Impulse); // ? Multiply force by mass for better effect

            StartCoroutine(RecoverFromKnockback());
        }
    }

    private IEnumerator RecoverFromKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);

        canMove = true; // ? Allow movement again after knockback
    }




    /*private IEnumerator DisableEnemyMovement(float duration)
    {
        yield return new WaitForSeconds(duration);
        canMove = true;
    }*/

    public void SetDazed(bool state)
    {
        isDazed = state;
        animator.SetBool("isDazed", isDazed);
        rb.velocity = Vector2.zero;
    }

    void FlipDirection(bool initial)
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
        if (!initial)
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
    }

    public void Die()
    {
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        Collider2D enemyCollider = GetComponent<Collider2D>();
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(deathDelay);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(gameObject);
    }
}




















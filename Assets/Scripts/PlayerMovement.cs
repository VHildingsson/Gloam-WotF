using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 10f;

    private bool isSprinting = false;
    private bool facingRight = true;
    private bool canMove = true; // ? Controls movement, including knockback restriction

    private Animator anim;

    [Header("Gravity Settings")]
    public float normalGravityScale = 5f;
    public float knockbackGravityScale = 2f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Wall Detection")]
    public float wallCheckDistance = 0.5f; // Distance to check for walls
    public LayerMask wallLayer; // Set this to the layer that contains your walls

    [Header("Audio Settings")]
    private AudioSource audioSource;
    private AudioSource gravityAudioSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip gravitySound;
    public float fadeDuration = 1.0f;
    private Coroutine fadeCoroutine;


    void Start()
    {
        canMove = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        gravityAudioSource = gameObject.AddComponent<AudioSource>();
        gravityAudioSource.loop = true;
        gravityAudioSource.playOnAwake = false;
        gravityAudioSource.clip = gravitySound;

        rb.gravityScale = normalGravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        if (!canMove)
        {
            Debug.Log("? Movement disabled!");
            return; // ? Prevent movement while knockback is active
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float move = Input.GetAxis("Horizontal");

        // Wall slide detection
        bool isTouchingWall = IsTouchingWall();

        if (Input.GetKeyDown(KeyCode.LeftShift) && move != 0)
        {
            isSprinting = true;
            anim.SetTrigger("startSprint");
            anim.SetBool("isSprinting", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
            anim.SetBool("isSprinting", false);
        }

        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;
        if (isTouchingWall && move != 0)
        {
            // Slide off the wall (apply a small force away from the wall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y); // Retain vertical speed
            if (move > 0)
            {
                rb.AddForce(Vector2.left * 2f); // Apply force to the left to slide off the wall
            }
            else if (move < 0)
            {
                rb.AddForce(Vector2.right * 2f); // Apply force to the right to slide off the wall
            }
        }
        else
        {
            rb.velocity = new Vector2(move * currentSpeed, rb.velocity.y);
        }

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("takeOff");
            audioSource.PlayOneShot(jumpSound, 0.1f);

        }

        anim.SetBool("isRunning", move != 0 && !isSprinting);
        anim.SetBool("isJumping", !isGrounded);
    }

    bool IsTouchingWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, wallCheckDistance, wallLayer);
        return hit.collider != null;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void AdjustGravity(bool duringKnockback)
    {
        rb.gravityScale = duringKnockback ? knockbackGravityScale : normalGravityScale;
    }

    public void DisableMovement(float duration)
    {
        StartCoroutine(DisableMovementCoroutine(duration));
    }

    IEnumerator DisableMovementCoroutine(float duration)
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        Debug.Log("? Movement disabled for " + duration + " seconds.");
        yield return new WaitForSeconds(duration);
        canMove = true;
        Debug.Log("? Movement re-enabled!");
    }

    public void EnableMovement()
    {
        canMove = true;
        Debug.Log("? Movement re-enabled.");
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    public void HandleGravityAudio(bool enteringGravityZone)
    {
        if (enteringGravityZone)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeGravityAudio(0.5f, true)); // Fade in
        }
        else
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeGravityAudio(0.0f, false)); // Fade out
        }
    }

    private IEnumerator FadeGravityAudio(float targetVolume, bool playIfStarting)
    {
        float startVolume = gravityAudioSource.volume;
        float elapsedTime = 0f;

        if (playIfStarting && !gravityAudioSource.isPlaying)
        {
            gravityAudioSource.Play();
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            gravityAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        gravityAudioSource.volume = targetVolume;

        if (targetVolume == 0)
        {
            gravityAudioSource.Stop(); // Ensure it fully stops when faded out
        }
    }

}












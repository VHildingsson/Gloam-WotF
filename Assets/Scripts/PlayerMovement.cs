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

    void Start()
    {
        canMove = true;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
        rb.velocity = new Vector2(move * currentSpeed, rb.velocity.y);

        if (move > 0 && !facingRight)
            Flip();
        else if (move < 0 && facingRight)
            Flip();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("takeOff");
        }

        anim.SetBool("isRunning", move != 0 && !isSprinting);
        anim.SetBool("isJumping", !isGrounded);
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
}












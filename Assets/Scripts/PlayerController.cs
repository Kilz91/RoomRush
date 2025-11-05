using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public float jumpForce;
    public LayerMask groundLayer;
    public Transform groundCheck;
    float horizontal;
    SpriteRenderer sr;
    Animator animator;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        animator.SetFloat("speed", Mathf.Abs(horizontal));
        animator.SetBool("IsGrounded", IsGrounded());
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        sr.flipX = horizontal < 0 ? true : false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 1f), CapsuleDirection2D.Horizontal, 0f, groundLayer);
    }

}
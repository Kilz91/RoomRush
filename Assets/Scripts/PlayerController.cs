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
        // Récupération des références locales nécessaires (sprite + animation)
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Mouvement horizontal: on applique la vitesse sur X uniquement
        // (velocity est l'API publique; linearVelocity était interne/legacy)
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
        animator.SetFloat("speed", Mathf.Abs(horizontal));
        animator.SetBool("IsGrounded", IsGrounded());
    }
    public void Move(InputAction.CallbackContext context)
    {
        // Callback du nouveau Input System: on lit l'axe horizontal (Vector2.x)
        horizontal = context.ReadValue<Vector2>().x;
        // Flip du sprite selon la direction (gauche = flipX true)
        sr.flipX = horizontal < 0 ? true : false;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            // Impulsion verticale quand le joueur est au sol
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    bool IsGrounded()
    {
        // Détection sol simple avec capsule Overlap (peut être affinée si besoin)
        return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 1f), CapsuleDirection2D.Horizontal, 0f, groundLayer);
    }

}
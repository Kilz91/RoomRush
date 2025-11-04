using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidBody;

    [SerializeField]
    private float _jumpForce;

    [SerializeField]
    private LayerMask _groundMask;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _deceleration;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics2D.Raycast(transform.position, Vector2.down, 1.1f, _groundMask))
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _rigidBody.AddForce(new Vector2(0, 1) * _jumpForce, ForceMode2D.Impulse);
            }
        }

        float direction = 0;

        if (Keyboard.current.aKey.isPressed)
        {
            direction = -1;
        } else if (Keyboard.current.dKey.isPressed)
        {
            direction = 1;
        }

        if(direction != 0)
        {
            _rigidBody.linearVelocity = new Vector2(direction * _speed, _rigidBody.linearVelocity.y);
        }else
        {
            _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x * _deceleration, _rigidBody.linearVelocity.y);
        }
    }
}
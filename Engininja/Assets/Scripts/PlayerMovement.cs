using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerMovement : MonoBehaviour
{
    public float maxPlayerSpeed;
    public float timeToMaxVelocity;
    public float playerJumpHeight;
    public LayerMask jumpableGround;
    public float playerGravity;

    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private InputHandler inputHandler;
    private float currentXVelocity = 0f;
    private Vector2 inputVector = Vector2.zero;
    private bool climbing = false;

    private enum AnimationState
    {
        idle,
        running,
        jumping,
        falling
    }

    private AnimationState state;

    void Start()
    {
        inputHandler = new InputHandler(GetComponent<PlayerInput>());
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    private float playerSpeed
    {
        get
        {
            if (climbing)
            {
                return maxPlayerSpeed * .5f;
            }
            return maxPlayerSpeed;
        }
    }

    private bool hasReleasedJump = false;

    void Update()
    {
        inputVector = inputHandler.getActionValue<Vector2>(InputHandlerActions.Move);
        // Check for roof crawling
        if (isUnderRoof())
        {
            if (climbing == false)
            {
                climbing = true;
                rb.gravityScale = 0f;
                hasReleasedJump = false;
            }
            if (inputVector.y <= 0f)
            {
                hasReleasedJump = true;
            }

            if (hasReleasedJump)
            {

                if (inputVector.y < 0f || inputVector.y > 0f)
                {
                    rb.gravityScale = playerGravity;
                }

            }
        }
        else
        {
            climbing = false;
            rb.gravityScale = playerGravity;
        }
        updateAnimationState();

        //currentXVelocity = rb.velocity.x;
    }

    private void FixedUpdate()
    {
        if (inputVector.y > .1f && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, playerJumpHeight);
        }
        else if (inputVector.y <= 0f && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        //float smoothXVelocity = Mathf.SmoothDamp(rb.velocity.x, inputVector.x * playerSpeed, ref currentXVelocity, timeToMaxVelocity);
        rb.velocity = new Vector2(inputVector.x * playerSpeed, rb.velocity.y);
    }

    private void updateAnimationState()
    {
        if (inputVector.x > 0.1f)
        {
            state = AnimationState.running;
            sprite.flipX = false;
        }
        else if (inputVector.x < -0.1f)
        {
            state = AnimationState.running;
            sprite.flipX = true;
        }
        else
        {
            state = AnimationState.idle;
        }

        if (rb.velocity.y < -.1f)
        {
            state = AnimationState.falling;
        }
        else if (rb.velocity.y > .1f)
        {
            state = AnimationState.jumping;
        }
        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private bool isUnderRoof()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.up, .1f, jumpableGround);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed = 7f;
    public float playerJumpHeight = 14f;
    public LayerMask jumpableGround;

    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private float xInput;

    private enum AnimationState
    {
        idle,
        running,
        jumping,
        falling
    }

    private AnimationState state;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(xInput * playerSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, playerJumpHeight);
        }

        updateAnimationState();
    }

    private void updateAnimationState()
    {
        if (xInput > 0f)
        {
            state = AnimationState.running;
            sprite.flipX = false;
        }
        else if (xInput < 0f)
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
}

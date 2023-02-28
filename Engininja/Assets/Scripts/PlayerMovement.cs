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
    private float lastJumpTime = float.NegativeInfinity;
    private float timeSinceLastFootstep;
    private bool leftFoot = false;
    public AudioSource leftFootAudio;
    public AudioSource rightFootAudio;
    private PlayerHealth playerHealth;
    public AudioSource jumpAudio;
    public AudioSource landAudio;

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
        Time.timeScale = 1;
        inputHandler = new InputHandler(GetComponent<PlayerInput>());
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth.isDead) return;
        inputVector = inputHandler.GetActionValue<Vector2>(InputHandlerActions.Move);
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        if (playerHealth.isDead) return;

        if (inputVector.y > .1f && IsGrounded())
        {
            lastJumpTime = Time.time;
            jumpAudio.Play();
            rb.velocity = new Vector2(rb.velocity.x, playerJumpHeight);
        }
        else if (inputVector.y <= 0f && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        float smoothXVelocity = Mathf.SmoothDamp(rb.velocity.x, inputVector.x * maxPlayerSpeed, ref currentXVelocity, timeToMaxVelocity);
        rb.velocity = new Vector2(inputVector.x * maxPlayerSpeed, rb.velocity.y);
        rb.velocity = new Vector2(smoothXVelocity, rb.velocity.y);
    }

    private void PlayWalkAudio()
    {
        if (timeSinceLastFootstep - Time.time > -0.3f || !IsGrounded()) { return; }
        timeSinceLastFootstep = Time.time;
        if (leftFoot)
        {
            leftFoot = false;
            leftFootAudio.pitch = Random.Range(0.9f, 1.1f);
            leftFootAudio.Play();
        }
        else
        {
            leftFoot = true;
            leftFootAudio.pitch = Random.Range(0.9f, 1.1f);
            rightFootAudio.Play();
        }
    }

    private void UpdateAnimationState()
    {
        if (inputVector.x > 0.1f)
        {
            PlayWalkAudio();
            state = AnimationState.running;
            sprite.flipX = false;
        }
        else if (inputVector.x < -0.1f)
        {
            PlayWalkAudio();
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
            if (IsGrounded())
            {
                landAudio.Play();
            }
        }
        else if (rb.velocity.y > .1f)
        {
            state = AnimationState.jumping;
        }
        anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        if (lastJumpTime - Time.time > -.5f) { return false; }
        return Physics2D.BoxCast(coll.bounds.center, Vector3.Scale(coll.bounds.size, new Vector3(0.9f, 1f, 0.9f)), 0f, Vector2.down, .1f, jumpableGround);
    }
}

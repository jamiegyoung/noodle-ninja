using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public Vector2 targetLocation;
    public bool atTargetLocation = false;
    private Collider2D coll;
    private Rigidbody2D rb;
    private PlayerDetection pd;
    public EnemyAI enemyAI;
    public float enemySpeed;
    private float currentXVelocity = 0f;
    public float timeToMaxVelocity;
    public float timeUntilContinueAfterSeen = 3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        enemyAI = GetComponent<EnemyAI>();
        pd = GetComponentInChildren<PlayerDetection>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float speed;
        if (pd.alertState == PlayerDetection.AlertState.Idle)
        {
            speed = enemySpeed;
        }
        else
        {
            speed = enemySpeed * 2;
        }
        Vector2 tmpLocation = new(targetLocation.x, targetLocation.y + 0.5f);
        if (coll.bounds.Contains(tmpLocation))
        {
            atTargetLocation = true;
            return;
        }
        if (pd.timeSinceLastSeenPlayer - Time.time > timeUntilContinueAfterSeen * -1)
        {
            return;
        }
        atTargetLocation = false;
        bool isFlipped = transform.position.x > targetLocation.x;
        //Debug.Log("setting flip due to movement to " + isFlipped);
        enemyAI.flipX = isFlipped;
        float directionMultiplier = isFlipped ? 1 : -1;
        float smoothXVel = Mathf.SmoothDamp(rb.velocity.x, speed * directionMultiplier * -1, ref currentXVelocity, timeToMaxVelocity);
        rb.velocity = new Vector2(smoothXVel, rb.velocity.y);
    }
}

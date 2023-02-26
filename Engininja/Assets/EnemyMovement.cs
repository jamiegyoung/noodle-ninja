using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float timeToMaxVelocity = 0.1f;
    public float timeUntilContinueAfterSeen = 3f;
    public GameObject roomsContainer;
    private List<Room> rooms;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        enemyAI = GetComponent<EnemyAI>();
        pd = GetComponentInChildren<PlayerDetection>();
        rooms = roomsContainer.GetComponentsInChildren<Room>().ToList();
    }

    void TraverseOwnRoom(float speed)
    {
        bool isFlipped = transform.position.x > targetLocation.x;
        //Debug.Log("setting flip due to movement to " + isFlipped);
        enemyAI.FlipX = isFlipped;
        float directionMultiplier = isFlipped ? 1 : -1;
        float smoothXVel = Mathf.SmoothDamp(rb.velocity.x, speed * directionMultiplier * -1, ref currentXVelocity, timeToMaxVelocity);
        rb.velocity = new Vector2(smoothXVel, rb.velocity.y);
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
        //Debug.Log(tmpLocation);
        if (coll.bounds.Contains(tmpLocation))
        {
            //enemyAI.FlipX = originalFlip ?? false;
            atTargetLocation = true;
            return;
        }

        // Wait a second to move after last seeing the player
        if (pd.timeSinceLastSeenPlayer - Time.time > timeUntilContinueAfterSeen * -1)
        {
            return;
        }

        // Get the current enemy room
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Room currentRoom = rooms.Find((room) => room.coll.bounds.Contains(currentPos));
        // Get the c the target location is in
        //Debug.Log(transform.position + " : " + tmpLocation);
        Room targetRoom = rooms.Find((room) => room.coll.bounds.Contains(tmpLocation));

        if (currentRoom == targetRoom || currentRoom == null || targetRoom == null)
        {
            TraverseOwnRoom(speed);
            return;
        }


        atTargetLocation = false;
    }
}

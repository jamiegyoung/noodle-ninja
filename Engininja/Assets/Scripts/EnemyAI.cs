using UnityEngine;
using static PlayerDetection;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour, Interactable
{
    //public Transform playerTransform;
    private float behaviourTimer;
    public bool isDead = false;
    private BoxCollider2D coll;
    private bool scoreDetectionFlag = false;
    private EnemyMovement enemyMovement;
    public float otherEnemiesLineOfSightDistance = 10f;
    public LayerMask enemyMask;
    public ScoreController scoreController;
    public PlayerDetection playerDetection;
    public AudioSource deathAudio;
    public SpriteRenderer alertSpriteRenderer;
    public LayerMask obstacleMask;
    public bool idleFlip;
    public List<PatrolLocation> patrolLocations;
    private bool _flipX;
    private int patrolLocationCounter = 0;
    private Animator anim;

    public bool FlipX
    {
        get
        {
            Debug.Log("Getting Flip");
            return _flipX;
        }
        set
        {
            behaviourTimer = Time.time;
            transform.localScale = new Vector3(value ? -1 : 1, transform.localScale.y, transform.localScale.z);
            _flipX = value;
        }
    }

    public bool IsInteractable => true;

    void Start()
    {
        anim = GetComponent<Animator>();
        //_flipX = flipX;
        enemyMovement = GetComponent<EnemyMovement>();
        if (idleFlip)
        {
            behaviourTimer = Time.time;
        }
        coll = GetComponent<BoxCollider2D>();
    }

    private void HandleBehaviour()
    {
        if (isDead || !enemyMovement.atTargetLocation) { return; }

        // Conditional behaviour dependent on the state
        // Score detection flag prevents duplicate subtracation of scores

        if (playerDetection.alertState == AlertState.Idle)
        {
            scoreDetectionFlag = false;
            IdleBehaviour();
        }
        else if (playerDetection.alertState == AlertState.Attacking)
        {
            if (scoreDetectionFlag == false)
            {
                scoreDetectionFlag = true;
                scoreController.AddScore(ScoreConditions.Detection);
            }
        }

        if (playerDetection.alertState == AlertState.Aware || playerDetection.alertState == AlertState.Attacking)
        {
            AggressiveBehaviour();
        }


    }

    private void AggressiveBehaviour()
    {
        Vector2 lastSeenLocation = playerDetection.lastSeenPlayerLocation;
        if (lastSeenLocation != null)
        {
            PatrolLocation lastLocationPatrolLocation = new();
            float y = RoundY(lastSeenLocation);
            lastLocationPatrolLocation.location = new Vector2(lastSeenLocation.x, y);
            if (behaviourTimer - Time.time < -1f && !playerDetection.hasVisionOfPlayer && enemyMovement.atTargetLocation)
            {
                FlipX = !FlipX;
            }
            enemyMovement.patrolLocation = lastLocationPatrolLocation;
            enemyMovement.patrolLocation.flipAtLocation = false;
        }
    }

    private static float RoundY(Vector2 lastSeenLocation)
    {
        if (lastSeenLocation.y < 1)
        {
            return -3;
        }
        else if (lastSeenLocation.y < 5)
        {
            return 1;
        }
        else { return -3; }
    }

    private void IdleBehaviour()
    {
        if (behaviourTimer - Time.time < -1f)
        {
            if (patrolLocations.Count > 0)
            {
                patrolLocationCounter++;
                if (patrolLocationCounter < patrolLocations.Count)
                {
                    enemyMovement.patrolLocation = patrolLocations[patrolLocationCounter];
                }
                else
                {
                    enemyMovement.patrolLocation = patrolLocations[0];
                    patrolLocationCounter = 0;
                }
            }
            else
            {
                FlipX = !FlipX;
            }
        }
    }

    private void UpdateOtherEnemies()
    {
        int viewAngleOffset = (transform.localScale.x == 1) ? 1 : -1;
        Vector2 angle = otherEnemiesLineOfSightDistance * viewAngleOffset * Vector2.right;

        RaycastHit2D[] hits = Physics2D.RaycastAll(coll.bounds.center, angle, otherEnemiesLineOfSightDistance, enemyMask + obstacleMask);
        //Debug.DrawRay(coll.bounds.center, angle, Color.red);
        foreach (RaycastHit2D hit in hits)
        {
            // Not an enemy
            if (!hit.collider) continue;
            PlayerDetection otherPlayerDetection = hit.collider.GetComponentInChildren<PlayerDetection>();
            if (!otherPlayerDetection) continue;
            if (otherPlayerDetection.alertState == AlertState.Attacking && playerDetection.alertState == AlertState.Idle)
            {
                Debug.Log("can see other enemy attacking");
                // If another enemy is attacking, this enemy becomes aware
                playerDetection.alertState = AlertState.Aware;
            }
        }
    }

    void Update()
    {
        HandleBehaviour();
        UpdateOtherEnemies();
    }

    public void Interact(GameObject interactor)
    {
        if (isDead == true)
        {
            return;
        }
        deathAudio.pitch = Random.Range(0.85f, 1.15f);
        deathAudio.Play();
        anim.SetBool("isDead", true);
        isDead = true;
        scoreController.AddScore(ScoreConditions.Kill);
        gameObject.layer = 9;
        //transform.rotation = Quaternion.Euler(0, 0, 90);
    }
}

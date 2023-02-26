using UnityEngine;
using static PlayerDetection;
using static ConvertToRad;
using UnityEditor.Experimental.GraphView;
using static ScoreController;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, Interactable
{
    //public Transform playerTransform;
    private float lastFlipped;
    private bool _isDead = false;
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
    private bool _flipX;
    public bool IsDead
    {
        get
        {
            return _isDead;
        }
    }

    public bool FlipX
    {
        get
        {
            Debug.Log("Getting Flip");
            return _flipX;
        }
        set
        {
            lastFlipped = Time.time;
            //rotationValue = (rotationValue + 180) % 360;
            Debug.Log("Setting Flip to " + value);
            //transform.rotation = Quaternion.Euler(0, rotationValue, 0);
            transform.localScale = new Vector3(value ? -1 : 1, transform.localScale.y, transform.localScale.z);
            _flipX = value;
        }
    }

    public bool IsInteractable => true;

    void Start()
    {
        //_flipX = flipX;
        enemyMovement = GetComponent<EnemyMovement>();
        if (idleFlip)
        {
            lastFlipped = Time.time;
        }
        coll = GetComponent<BoxCollider2D>();
    }

    private void HandleBehaviour()
    {
        if (IsDead || !enemyMovement.atTargetLocation) { return; }

        // Conditional behaviour dependent on the state
        // Score detection flag prevents duplicate subtracation of scores
        switch (playerDetection.alertState)
        {
            case AlertState.Idle:
                scoreDetectionFlag = false;
                IdleBehaviour();
                break;
            case AlertState.Aware:
                scoreDetectionFlag = false;
                //idleBehaviour();
                break;
            case AlertState.Attacking:
                if (scoreDetectionFlag == false)
                {
                    scoreDetectionFlag = true;
                    scoreController.AddScore(ScoreConditions.Detection);
                }
                //idleBehaviour();
                break;
        }
    }

    private void IdleBehaviour()
    {
        if (idleFlip && lastFlipped - Time.time < -5f)
        {
            FlipX = !FlipX;
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
        deathAudio.pitch = Random.Range(0.85f, 1.15f);
        deathAudio.Play();
        if (_isDead == true)
        {
            return;
        }
        _isDead = true;
        scoreController.AddScore(ScoreConditions.Kill);
        gameObject.layer = 9;
        transform.rotation = Quaternion.Euler(0, 0, 90);
    }
}

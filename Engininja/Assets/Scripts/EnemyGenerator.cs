using UnityEngine;

[System.Serializable]
public struct Enemy
{
    public Vector2 spawnLocation;
    public Vector2 patrolDestination;
    public bool flipX;
    public float waitTime;
    public bool idleFlip;
    public Vector2 targetLocation;
}

public class EnemyGenerator : MonoBehaviour
{
    public Enemy[] enemies;
    public GameObject enemyTemplate;
    public LayerMask enemyMask;
    public ScoreController scoreController;
    public Transform playerTransform;
    public PlayerHealth playerHealth;
    private GameObject[] enemyGameObjects;
    //private ActionState[] enemyPlayerVision;
    // Start is called before the first frame update
    void Start()
    {
        enemyGameObjects = new GameObject[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i];
            //GameObject duplicate = enemyTemplate;
            GameObject duplicate = Instantiate(enemyTemplate);
            duplicate.GetComponent<Transform>().position = enemy.spawnLocation;
            EnemyAI enemyAI = duplicate.GetComponent<EnemyAI>();
            enemyAI.enemyMask = enemyMask;
            enemyAI.scoreController = scoreController;
            enemyAI.idleFlip = enemy.idleFlip;
            enemyAI.FlipX = enemy.flipX;
            PlayerDetection playerDetection = duplicate.GetComponentInChildren<PlayerDetection>();
            playerDetection.playerHealth = playerHealth;
            playerDetection.playerTransform = playerTransform;
            EnemyMovement enemyMovement = duplicate.GetComponentInChildren<EnemyMovement>();
            enemyMovement.targetLocation = enemy.targetLocation;
            enemyGameObjects[i] = duplicate;
        }
    }

    /// <summary>
    /// Checks if the player is currently seen by any enemies and is being attacked
    /// Mainly used for disabling hiding spots
    /// </summary>
    /// <returns>Whether the player is currently in vision and being attacked by</returns>
    public bool playerIsSeenAndBeingAttacked()
    {
        for (int i = 0; i < enemyGameObjects.Length; i++)
        {
            GameObject enemyGameObject = enemyGameObjects[i];
            PlayerDetection playerDetection = enemyGameObject.GetComponentInChildren<PlayerDetection>();
            if (playerDetection.hasVisionOfPlayer && playerDetection.alertState == PlayerDetection.AlertState.Attacking)
            {
                Debug.Log("Player found on interaction check");
                return true;
            }
        }
        Debug.Log("Player not found on interaction check");
        return false;
    }
}

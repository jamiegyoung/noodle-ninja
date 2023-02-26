using System.Collections.Generic;
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
    private List<GameObject> enemyGameObjects = new List<GameObject>();

    void Start()
    {
        Debug.Log("Creating " + enemies.Length + " enemies");
        for (int i = 0; i < enemies.Length; i++)
        {
            Enemy enemy = enemies[i];
            GameObject duplicate = enemyTemplate;
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
            enemyGameObjects.Add(Instantiate(duplicate));
        }
    }

    /// <summary>
    /// Checks if the player is currently seen by any enemies and is being attacked
    /// Mainly used for disabling hiding spots
    /// </summary>
    /// <returns>Whether the player is currently in vision and being attacked by</returns>
    public bool playerIsSeenAndBeingAttacked()
    {
        for (int i = 0; i < enemyGameObjects.Count; i++)
        {
            GameObject enemyGameObject = enemyGameObjects[i];
            PlayerDetection playerDetection = enemyGameObject.GetComponentInChildren<PlayerDetection>();
            if (playerDetection.hasVisionOfPlayer && playerDetection.alertState == PlayerDetection.AlertState.Attacking)
            {
                return true;
            }
        }
        return false;
    }
}

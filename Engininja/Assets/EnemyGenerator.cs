using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[System.Serializable]
public struct Enemy
{
    public Vector2 spawnLocation;
    public Vector2 patrolDestination;
    public bool flipX;
    public float waitTime;
    public bool idleFlip;
}

public class EnemyGenerator : MonoBehaviour
{
    public Enemy[] enemies;
    public GameObject enemyTemplate;
    public LayerMask enemyMask;
    public ScoreController scoreController;
    public Transform playerTransform;
    public PlayerHealth playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Enemy enemy in enemies)
        {
            //GameObject duplicate = enemyTemplate;
            GameObject duplicate = Instantiate(enemyTemplate);
            duplicate.GetComponent<Transform>().position = enemy.spawnLocation;
            EnemyAI enemyAI = duplicate.GetComponent<EnemyAI>();
            enemyAI.enemyMask = enemyMask;
            enemyAI.scoreController = scoreController;
            enemyAI.idleFlip = enemy.idleFlip;
            enemyAI.flipX = enemy.flipX;
            PlayerDetection playerDetection = duplicate.GetComponentInChildren<PlayerDetection>();
            playerDetection.playerHealth = playerHealth;
            playerDetection.playerTransform = playerTransform;
            //Instantiate(duplicate);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

[System.Serializable]
public struct PatrolLocation
{
    public Vector2 location;
    public bool flipAtLocation;
}

[System.Serializable]
public struct Enemy
{
    public Vector2 spawnLocation;
    public bool flipX;
    public float waitTime;
    public List<PatrolLocation> patrolLocations;
}

public class EnemyGenerator : MonoBehaviour
{
    public float enemyLightBrightness = 1.5f;
    public Enemy[] enemies;
    public GameObject enemyTemplate;
    public LayerMask enemyMask;
    public ScoreController scoreController;
    public Transform playerTransform;
    public PlayerHealth playerHealth;
    private readonly List<GameObject> enemyGameObjects = new();
    public GameObject roomsContainer;
    public LayerMask interactableMask;
    public Switch lightSwitch;
    private Light2D[] enemyLights;


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
            enemyAI.FlipX = enemy.flipX;
            enemyAI.patrolLocations = enemy.patrolLocations;
            PlayerDetection playerDetection = duplicate.GetComponentInChildren<PlayerDetection>();
            playerDetection.playerHealth = playerHealth;
            playerDetection.playerTransform = playerTransform;
            playerDetection.generator = this;
            EnemyMovement enemyMovement = duplicate.GetComponentInChildren<EnemyMovement>();
            enemyMovement.patrolLocation = enemy.patrolLocations[0];
            enemyMovement.roomsContainer = roomsContainer;
            enemyMovement.interactableMask = interactableMask;
            enemyGameObjects.Add(Instantiate(duplicate, gameObject.transform));
        }
        enemyLights = gameObject.GetComponentsInChildren<Light2D>();
    }

    public void OnGunShot(Vector2 location)
    {
        foreach (GameObject enemyGameObject in enemyGameObjects)
        {
            PlayerDetection playerDetection = enemyGameObject.GetComponentInChildren<PlayerDetection>();
            Vector2 target = new Vector2(location.x, location.y);
            Room[] rooms = roomsContainer.GetComponentsInChildren<Room>();
            bool targetFlag = false;
            foreach (Room room in rooms)
            {
                if (room.coll.bounds.Contains(target))
                {
                    targetFlag = true;
                }
            }
            if (targetFlag)
            {
                playerDetection.lastSeenPlayerLocation = target;
            }
            playerDetection.alertCounter = PlayerDetection.MAX_ALERT;
            playerDetection.alertState = PlayerDetection.AlertState.Aware;
        }
    }

    public IEnumerator SwitchEnemyLights()
    {
        yield return new WaitForSeconds(1f);
        foreach (Light2D light in enemyLights)
        {
            light.intensity = light.intensity == 0f ? enemyLightBrightness : 0f;
            yield return new WaitForSeconds(0.02f);
        }
    }

    /// <summary>
    /// Checks if the player is currently seen by any enemies and is being attacked
    /// Mainly used for disabling hiding spots
    /// </summary>
    /// <returns>Whether the player is currently in vision and being attacked by</returns>
    public bool PlayerIsSeenAndBeingAttacked()
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

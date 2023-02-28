using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static ConvertToRad;

// modified version of https://stackoverflow.com/a/643438
// Removed wrapping and added previous
public static class Extensions
{
    public static T Next<T>(this T src) where T : struct
    {

        T[] Arr = GetArrFromEnum(src);
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[j - 1] : Arr[j];
    }

    public static T Previous<T>(this T src) where T : struct
    {
        T[] Arr = GetArrFromEnum(src);
        int j = Array.IndexOf<T>(Arr, src) - 1;
        return (j == -1) ? Arr[0] : Arr[j];
    }

    private static T[] GetArrFromEnum<T>(T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));
        return (T[])Enum.GetValues(src.GetType());
    }
}

public class PlayerDetection : MonoBehaviour
{
    private BoxCollider2D coll;

    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float lineOfSightDistance = 5f;
    public int viewAngle = 50;
    public int castStep = 2;
    public float rotationStep = .02f;
    public bool hasVisionOfPlayer = false;
    public AlertState alertState;
    public Transform playerTransform;
    public PlayerHealth playerHealth;
    public AudioSource awareAudio;
    public AudioSource attackingAudio;
    public AudioSource bulletMiss;
    public Sprite awareSprite;
    public Sprite attackingSprite;
    public SpriteRenderer alertStateSprite;
    public EnemyGenerator generator;
    public Animator parentAnimator;

    public int alertCounter = 0;
    private float lastAlertTime = 0;
    private float timeSinceLastShot = 0f;
    private AudioSource gunShotSource;
    private const float MIN_GUN_AUDIO_DISTANCE = 1f;
    private const float MAX_GUN_AUDIO_DISTANCE = 30f;
    public const int MAX_ALERT = 250;
    private const int MIN_ALERT = 0;
    private const float REACTION_TIME = .5f;
    private const float TIME_BETWEEN_SHOTS = 3f;
    private const int ALERT_INCREASE_SPEED = 8;
    private const float LOS_VERTICAL_OFFSET = .5f;
    private float timeSeen = 0f;
    public float timeSinceLastSeenPlayer = 0f;
    public Vector3 lastSeenPlayerLocation;

    public enum AlertState
    {
        Idle,
        Aware,
        Attacking
    }

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        gunShotSource = GetComponent<AudioSource>();
    }

    private void Shoot(bool miss)
    {
        if (timeSinceLastShot - Time.time < TIME_BETWEEN_SHOTS * -1)
        {
            parentAnimator.SetTrigger("gunShot");
            generator.OnGunShot(transform.position);
            timeSinceLastShot = Time.time;
            // Change volume based on distance
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            if (dist < MIN_GUN_AUDIO_DISTANCE)
            {
                gunShotSource.volume = 1;
            }
            else if (dist > MAX_GUN_AUDIO_DISTANCE)
            {
                gunShotSource.volume = 0;
            }
            else
            {
                // Cur Dist / Max Dist
                gunShotSource.volume = 1 - ((dist - MIN_GUN_AUDIO_DISTANCE) / (MAX_GUN_AUDIO_DISTANCE - MIN_GUN_AUDIO_DISTANCE));
            }
            gunShotSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            gunShotSource.Play();
            // 1 in 10 chance of missing
            if (UnityEngine.Random.Range(0, 10) != 0 && !miss)
            {
                Debug.Log("shoot");
                playerHealth.Damage();
            }
            else
            {
                bulletMiss.PlayDelayed(.2f);
            }
        }
    }

    private bool ForCheck(int i, int viewAngleOffset)
    {
        if (viewAngleOffset < 0)
        {
            return i > viewAngle / 2 * viewAngleOffset;
        }
        else
        {
            return i < viewAngle / 2 * viewAngleOffset;
        }
    }

    private void UpdateAlertCounter()
    {
        // 50 buffer for alert going down
        if (hasVisionOfPlayer && alertCounter < MAX_ALERT + 50)
        {
            // goes up 3 times quicker than down
            alertCounter += ALERT_INCREASE_SPEED;
            lastAlertTime = Time.time;
        }
        else if (!hasVisionOfPlayer && alertCounter > MIN_ALERT)
        {
            alertCounter--;
        }

        // increment the state if hit max
        if (alertCounter > MAX_ALERT && alertState != AlertState.Attacking)
        {
            // make sure we don't loop up and down
            alertCounter = MIN_ALERT + 5;
            alertState = alertState.Next();
            switch (alertState)
            {
                case AlertState.Attacking:
                    attackingAudio.Play(); break;
                case AlertState.Aware:
                    awareAudio.Play(); break;
            }
        }

        // decrement if down
        if (alertCounter <= MIN_ALERT)
        {
            //Debug.Log(Time.time - lastAlertTime);
            if (Time.time - lastAlertTime > 12f && alertState != AlertState.Idle)
            {
                alertCounter = MAX_ALERT;
                alertState = alertState.Previous();
            }
        }
    }

    private void UpdateAlertVisuals()
    {
        alertStateSprite.flipX = playerTransform.localScale.x > 0;
        switch (alertState)
        {
            case AlertState.Idle:
                alertStateSprite.enabled = false;
                break;
            case AlertState.Attacking:
                alertStateSprite.enabled = true;
                alertStateSprite.sprite = attackingSprite;
                break;
            case AlertState.Aware:
                alertStateSprite.enabled = true;
                alertStateSprite.sprite = awareSprite;
                break;
        }

    }


    private void UpdateAlert()
    {
        UpdateAlertCounter();
        UpdateAlertVisuals();
        HandleLineOfSight();
    }



    private void HandleLineOfSight()
    {
        int viewAngleOffset = (int)transform.parent.transform.localScale.x;
        // Prevent crashes
        if (viewAngle < 1 || castStep < 1) return;
        hasVisionOfPlayer = false;
        // Cast a cone of a given degree
        for (int i = viewAngle / 2 * -viewAngleOffset; ForCheck(i, viewAngleOffset); i += castStep * viewAngleOffset)
        {
            // No need to raycast if the player has been found
            if (hasVisionOfPlayer) break;
            //float offset = parentRotation.y * 180;
            Vector2 angle = new(
                lineOfSightDistance * Mathf.Sin(ConvertToRad.Convert(i + Quaternion.Angle(Quaternion.Euler(0, 0, 90), transform.rotation) * viewAngleOffset)),
                lineOfSightDistance * Mathf.Cos(ConvertToRad.Convert(i + Quaternion.Angle(Quaternion.Euler(0, 0, 90), transform.rotation) * viewAngleOffset))
                );
            Vector3 origin = new(
                    coll.bounds.center.x,
                    coll.bounds.center.y + LOS_VERTICAL_OFFSET,
                    coll.bounds.center.z
                    );
            RaycastHit2D raycastHit = Physics2D.Raycast(origin
                ,
                angle, lineOfSightDistance, playerMask + obstacleMask
               );
            if (raycastHit.collider && raycastHit.collider.gameObject.name == "Player")
            {
                hasVisionOfPlayer = true;
                Debug.DrawRay(origin, angle, Color.red);
                Vector3 targetPos = raycastHit.collider.gameObject.transform.position;
                Quaternion target = Quaternion.LookRotation(
                    new Vector3(targetPos.x, targetPos.y + .5f, 0) - transform.position, transform.TransformDirection(Vector3.up)
                    );
                this.lastSeenPlayerLocation = targetPos;
                transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(0, 0, target.z, target.w), rotationStep);
                timeSinceLastSeenPlayer = Time.time;
            }
            else
            {
                Debug.DrawRay(origin, angle);
            }
        }
        if (hasVisionOfPlayer == false)
        {
            timeSeen = Time.time;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), rotationStep);
            // Panic shot, attacking but no vision
            if (UnityEngine.Random.Range(0, 400) == 0 && alertState == AlertState.Attacking)
            {
                Debug.Log("panic shot");
                Shoot(true);
            }
        }
        //Can only shoot when attacking and has vision
        else if (alertState == AlertState.Attacking)
        {
            if (timeSeen - Time.time < (REACTION_TIME * -1) - UnityEngine.Random.Range(0f, 1f))
            {
                Debug.Log("Shooting with vision");
                Shoot(false);

            }
        }
    }

    private void OnDrawGizmos()
    {
        GUIStyle style = new();
        style.normal.textColor = Color.green;
        style.fontSize = 18;
        style.alignment = TextAnchor.MiddleCenter;
        style.border = new RectOffset(10, 10, 10, 10);
        Handles.Label(new Vector2(transform.position.x, transform.position.y + .5f), alertCounter.ToString() + " : " + alertState.ToString(), style);
    }

    void FixedUpdate()
    {
        if (GetComponentInParent<EnemyAI>().isDead == true)
        {
            hasVisionOfPlayer = false;
            alertState = AlertState.Idle;
            return;
        }
        UpdateAlert();
    }
}

using System;
using UnityEditor;
using UnityEngine;
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
    private bool hasVisionOfPlayer = false;
    private int alertCounter = 0;
    private float lastAlertTime = 0;
    public AlertState alertState;
    public Transform playerTransform;
    public PlayerHealth playerHealth;
    private float timeSinceLastShot = 0f;
    private AudioSource gunShotSource;
    private const float MIN_GUN_AUDIO_DISTANCE = 1f;
    private const float MAX_GUN_AUDIO_DISTANCE = 30f;
    private const int MAX_ALERT = 250;
    private const int MIN_ALERT = 0;

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
        if (timeSinceLastShot - Time.time < -3f)
        {
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
            Debug.Log(gunShotSource.volume);

            gunShotSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            gunShotSource.Play();
            // 1 in 10 chance of missing
            if (UnityEngine.Random.Range(0, 10) != 0 && !miss)
            {
                Debug.Log("shoot");
                playerHealth.Damage();
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
            alertCounter += 3;
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
        }

        // decrement if down
        if (alertCounter <= MIN_ALERT)
        {
            //Debug.Log(Time.time - lastAlertTime);
            if (Time.time - lastAlertTime > 30f && alertState != AlertState.Idle)
            {
                alertCounter = MAX_ALERT;
                alertState = alertState.Previous();
            }
        }
    }

    //private void UpdateAlertState()
    //{
    //    if (alertCounter < (int)AlertState.Aware)
    //    {
    //        alertState = AlertState.Idle;
    //        return;
    //    }
    //    else if (alertCounter < (int)AlertState.Aware)
    //    {
    //        alertState = AlertState.Aware;
    //    }
    //    else if (alertCounter < (int)AlertState.Attacking)
    //    {
    //        alertState = AlertState.Aware;
    //    }
    //    else
    //    {
    //        alertState = AlertState.Attacking;
    //    }
    //    // Set the last aware time if was aware
    //    lastAlertTime = Time.time;

    //}


    private void UpdateAlert()
    {
        UpdateAlertCounter();
        //UpdateAlertState();
        if (GetComponentInParent<EnemyAI>().IsDead == true) { return; }
        HandleLineOfSight();
    }



    private void HandleLineOfSight()
    {
        Quaternion parentRotation = transform.parent.transform.rotation;
        int viewAngleOffset = (parentRotation.y == 0) ? 1 : -1;
        // Prevent crashes
        if (viewAngle < 1 || castStep < 1) return;
        hasVisionOfPlayer = false;
        // Cast a cone of a given degree
        for (int i = viewAngle / 2 * -viewAngleOffset; ForCheck(i, viewAngleOffset); i += castStep * viewAngleOffset)
        {
            // No need to raycast if the player has been found
            if (hasVisionOfPlayer) break;
            float offset = parentRotation.y * 180;
            Vector2 angle = new(
                lineOfSightDistance * Mathf.Sin(ConvertToRad.Convert(offset + i + Quaternion.Angle(Quaternion.Euler(0, 0, 90), transform.rotation))),
                lineOfSightDistance * Mathf.Cos(ConvertToRad.Convert(offset + i + Quaternion.Angle(Quaternion.Euler(0, 0, 90), transform.rotation)))
                );

            RaycastHit2D raycastHit = Physics2D.Raycast(coll.bounds.center, angle, lineOfSightDistance, playerMask + obstacleMask);
            if (raycastHit.collider && raycastHit.collider.gameObject.name == "Player")
            {
                hasVisionOfPlayer = true;
                Debug.DrawRay(coll.bounds.center, angle, Color.red);
                Quaternion target = Quaternion.LookRotation(
                    raycastHit.collider.gameObject.transform.position - transform.position, transform.TransformDirection(Vector3.up)
                    );

                transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(0, 0, target.z, target.w), rotationStep);

            }
            else
            {
                Debug.DrawRay(coll.bounds.center, angle);
            }
        }
        if (hasVisionOfPlayer == false)
        {
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
            Debug.Log("Shooting with vision");
            Shoot(false);
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
        if (GetComponentInParent<EnemyAI>().IsDead == true) { return; }
        UpdateAlert();
    }

    void Update()
    {

    }
}

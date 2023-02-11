using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    private const int MAX_ALERT = 100;
    private const int MIN_ALERT = 0;

    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    private float ConvertToRad(float num) => num * (Mathf.PI / 180);

    private bool forCheck(int i, int viewAngleOffset)
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

    void FixedUpdate()
    {
        updateAlert();
    }

    private void updateAlert()
    {
        if (hasVisionOfPlayer && alertCounter < MAX_ALERT)
        {
            alertCounter++;
        }
        else if (!hasVisionOfPlayer && alertCounter > MIN_ALERT)
        {
            alertCounter--;
        }
    }

    private void OnDrawGizmos()
    {
        GUIStyle style= new GUIStyle();
        style.normal.textColor = Color.green;
        style.fontSize = 18;
        style.alignment = TextAnchor.MiddleCenter;
        style.border = new RectOffset(10, 10, 10, 10);
        Handles.Label(new Vector2(transform.position.x, transform.position.y + .5f), alertCounter.ToString(), style);
    }

    void Update()
    {
        handleLineOfSight();

    }

    private void handleLineOfSight()
    {
        Quaternion parentRotation = transform.parent.transform.rotation;
        int viewAngleOffset = (parentRotation.y == 0) ? 1 : -1;
        // Prevent crashes
        if (viewAngle < 1 || castStep < 1) return;
        hasVisionOfPlayer = false;
        // Cast a cone of a given degree
        for (int i = viewAngle / 2 * -viewAngleOffset; forCheck(i, viewAngleOffset); i += castStep * viewAngleOffset)
        {
            // No need to raycast if the player has been found
            if (hasVisionOfPlayer) break;
            float offset = parentRotation.y * 180;
            Vector2 angle = new Vector2(
                lineOfSightDistance * Mathf.Sin(ConvertToRad(offset + i + Quaternion.Angle(Quaternion.Euler(0, 0, 90), transform.rotation))),
                lineOfSightDistance * Mathf.Cos(ConvertToRad(offset + i + Quaternion.Angle(Quaternion.Euler(0, 0, 90), transform.rotation)))
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

        }
    }
}

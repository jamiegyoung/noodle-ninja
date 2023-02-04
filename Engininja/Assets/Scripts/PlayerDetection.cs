using System.Collections;
using System.Collections.Generic;
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

    void Update()
    {
        Quaternion parentRotation = transform.parent.transform.rotation;
        int viewAngleOffset = (parentRotation.y == 0) ? 1 : -1;
        // Prevent crashes
        if (viewAngle < 1 || castStep < 1) return;
        bool found = false;
        // Cast a cone of a given degree
        for (int i = viewAngle / 2 * -viewAngleOffset; forCheck(i, viewAngleOffset); i += castStep * viewAngleOffset)
        {
            // No need to raycast if the player has been found
            if (found) break;
            float offset = parentRotation.y * 180;
            Vector2 angle = new Vector2(
                lineOfSightDistance * Mathf.Sin(ConvertToRad(offset + i + Quaternion.Angle(Quaternion.Euler(0, 0, 90), transform.rotation))),
                lineOfSightDistance * Mathf.Cos(ConvertToRad(offset + i + Quaternion.Angle(Quaternion.Euler(0, 0, 90), transform.rotation)))
                );

            RaycastHit2D raycastHit = Physics2D.Raycast(coll.bounds.center, angle, lineOfSightDistance, playerMask + obstacleMask);
            if (raycastHit.collider && raycastHit.collider.gameObject.name == "Player")
            {
                found = true;
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
        if (found == false)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), rotationStep);
        }

    }
}

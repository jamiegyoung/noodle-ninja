using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private BoxCollider2D coll;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float lineOfSightDistance = 20f;
    public int viewAngle = 20;
    public int castStep = 2;


    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    private float ConvertToRad(float num) => num * (Mathf.PI / 180);

    void Update()
    {
        Vector3 los = new Vector3(lineOfSightDistance, coll.bounds.size.y * 1.5f);
        // Prevent crashes
        if (viewAngle < 1 || castStep < 1) return;
        // Cast a cone of a given degree
        for (int i = viewAngle / 2 * -1; i < viewAngle / 2; i += castStep)
        {
            Vector2 t = new Vector2(
                coll.bounds.center.x + lineOfSightDistance * Mathf.Sin(ConvertToRad(i + 90)),
                coll.bounds.center.y + lineOfSightDistance * Mathf.Cos(ConvertToRad(i + 90))
                );
            RaycastHit2D raycastHit = Physics2D.Raycast(coll.bounds.center, t, lineOfSightDistance, playerMask + obstacleMask);
            if (raycastHit.collider && raycastHit.collider.gameObject.name == "Player")
            {
                Debug.Log(raycastHit.collider.gameObject.name);
                Debug.DrawRay(coll.bounds.center, t, Color.red);

            }
            else
            {
                Debug.DrawRay(coll.bounds.center, t);

            }
        }

    }
}

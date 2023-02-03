using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody2D target;
    public float smoothTime = 0.1f;
    public float yOffset = 2f;
    public float aheadAmount = 0.2f;

    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        float xGoal = target.position.x + target.velocity.x * aheadAmount;
        Vector3 goal = new Vector3(xGoal, target.position.y + yOffset, -10);
        transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    private void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, -10);
    }
}

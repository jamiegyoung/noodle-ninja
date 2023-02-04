using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //public Transform playerTransform;
    private float lastFlipped;
    private float rotationValue = 0;


    void Start()
    {
        lastFlipped = Time.time;
        //headTransform = transform.GetChild(0);
    }

    void Update()
    {
        //Debug.Log(headTransform.rotation.z);
        //Debug.Log(Quaternion.Angle(Quaternion.Euler(0, 0, 0), headTransform.rotation));
        //float headAngle = Quaternion.Angle(Quaternion.Euler(0, 0, 0), headTransform.rotation);
    
        if (lastFlipped - Time.time < -5f)
        {
            lastFlipped = Time.time;
            rotationValue = (rotationValue + 180) % 360;
            transform.rotation = Quaternion.Euler(0, rotationValue, 0);
        }
        //Quaternion(0, 0, 0, 1)
        //Quaternion(0, 1, 0, 0)
    }
}

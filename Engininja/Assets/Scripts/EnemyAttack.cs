using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public PlayerDetection playerDetection;
    private Collider2D coll;
    private LayerMask playerMask;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        Vector2 offset = new Vector3(0, 0, transform.rotation.eulerAngles.z);
        //Debug.DrawRay(coll.bounds.center, offset, Color.red);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}

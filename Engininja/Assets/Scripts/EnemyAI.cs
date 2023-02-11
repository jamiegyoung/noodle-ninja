using UnityEngine;
using static PlayerDetection;

public class EnemyAI : MonoBehaviour
{
    //public Transform playerTransform;
    private float lastFlipped;
    private float rotationValue = 0;
    public PlayerDetection playerDetection;
    private bool _isDead = false;
    public bool IsDead
    {
        get
        {
            return _isDead;
        }
        set
        {

            if (value == true)
            {
                gameObject.layer = 0;
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            _isDead = value;
        }
    }


    void Start()
    {
        lastFlipped = Time.time;
        //headTransform = transform.GetChild(0);
    }

    private void HandleBehaviour()
    {
        if (IsDead) { return; }

        switch (playerDetection.alertState)
        {
            case AlertState.Idle:
                IdleBehaviour();
                break;
            case AlertState.Aware:
                //idleBehaviour();
                break;
            case AlertState.Attacking:
                //idleBehaviour();
                break;
        }
    }

    private void IdleBehaviour()
    {
        if (lastFlipped - Time.time < -5f)
        {
            lastFlipped = Time.time;
            rotationValue = (rotationValue + 180) % 360;
            transform.rotation = Quaternion.Euler(0, rotationValue, 0);
        }
    }

    void Update()
    {
        HandleBehaviour();
    }
}

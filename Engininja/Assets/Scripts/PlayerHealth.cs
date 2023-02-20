using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject deathCanvas;
    public bool isDead;
    public int health = 3;
    private float timeSinceLastDamage = 0f;

    private void FixedUpdate()
    {
        // Health Regeneration
        if (timeSinceLastDamage - Time.time < -10f && health < 3)
        {
            health += 1;
        }
    }

    void Update()
    {
        if (health <= 0 && !isDead)
        {
            deathCanvas.SetActive(true);
        }
    }

    public void Damage()
    {
        if (health > 0)
        {
            timeSinceLastDamage = Time.time;
            health -= 1;
        }
    }

    public override string ToString() => String.Format("{0}HP", health.ToString());
}

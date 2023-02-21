using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    // Update is called once per frame
    void Update()
    {
        float health = playerHealth.health;
        heart1.SetActive(health > 0);
        heart2.SetActive(health > 1);
        heart3.SetActive(health > 2);
    }
}

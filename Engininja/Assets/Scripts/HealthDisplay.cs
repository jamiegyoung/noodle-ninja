using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    public PlayerHealth playerHealth;

    private void Start()
    {
        textMeshPro= GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = playerHealth.ToString();
    }
}

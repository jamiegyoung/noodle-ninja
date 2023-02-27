using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class getAttackedScore : MonoBehaviour
{
    public ScoreController scoreController;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.SetText(scoreController.detectionCounter + " ENEMIES ATTEMPTED TO KILL YOU");
    }
}

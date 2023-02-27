using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class getBlindScore : MonoBehaviour
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
        text.SetText("YOU BLINDED " + scoreController.flashbangCounter + " ENEMIES");
    }
}

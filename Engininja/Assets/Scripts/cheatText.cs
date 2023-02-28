using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class cheatText : MonoBehaviour
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
        if (scoreController.cheated)
        {
            text.SetText("BECAUSE YOU SKIPPED THE GAME");
        }
        else
        {
            text.SetText("");
        }
    }
}

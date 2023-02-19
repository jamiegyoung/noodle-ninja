using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;

public enum ScoreConditions
{
    Kill = 1000,
    Detection = -300,
    Flashbang = 50
}

public class ScoreController : MonoBehaviour
{
    private long score;
    private TextMeshPro textMeshPro;

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    public long AddScore(ScoreConditions condition)
    {
        score += (long)condition;
        textMeshPro.text = score.ToString();
        return score;
    }

    public long GetScore()
    {
        return score;
    }
}

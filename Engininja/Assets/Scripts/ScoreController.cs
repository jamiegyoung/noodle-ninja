using TMPro;
using UnityEngine;

public enum ScoreConditions
{
    Kill = 1000,
    Detection = -300,
    Flashbang = 50,
    Cheating = -1000000
}

public class ScoreController : MonoBehaviour
{
    private long score;
    private TextMeshProUGUI textMeshPro;
    public int killCounter = 0;
    public int flashbangCounter = 0;
    public int detectionCounter = 0;
    public bool cheated = false;

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public long AddScore(ScoreConditions condition)
    {
        switch (condition)
        {
            case ScoreConditions.Kill:
                score += (long)condition;
                killCounter++;
                break;
            case ScoreConditions.Detection:
                score += (long)condition;
                detectionCounter++;
                break;
            case ScoreConditions.Flashbang:
                score += (long)condition;
                flashbangCounter++;
                break;
            case ScoreConditions.Cheating:
                cheated = true;
                break;
        }

        textMeshPro.text = score.ToString();
        return score;
    }

    public long GetScore()
    {
        return score;
    }
}

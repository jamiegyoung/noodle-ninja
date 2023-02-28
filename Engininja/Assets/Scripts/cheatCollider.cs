using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cheatCollider : MonoBehaviour
{
    public GameObject player;
    public ScoreController scoreController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            scoreController.AddScore(ScoreConditions.Cheating);
        }
    }
}

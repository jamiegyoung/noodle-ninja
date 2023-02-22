using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour, Interactable
{

    public EnemyGenerator enemyGenerator;

    public bool IsInteractable => !enemyGenerator.playerIsSeenAndBeingAttacked();

    public void Interact()
    {
        Debug.Log("HIDING");
    }
}

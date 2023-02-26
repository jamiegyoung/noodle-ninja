using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour, Interactable
{
    public GameObject player;
    public Vector2 targetLocation;

    public bool IsInteractable => true;

    public void Interact(GameObject interactor)
    {
        interactor.transform.position = targetLocation;
    }
}

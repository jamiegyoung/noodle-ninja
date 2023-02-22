using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour, Interactable
{
    public GameObject player;
    private Transform playerTransform;
    private SpriteRenderer sprite;
    public Vector2 targetLocation;

    public void Interact(InteractionInformer interactionInformer)
    {
        bool isFlipped = sprite.flipX;
        //const flipped = playerTransform.localScale.
        playerTransform.position = targetLocation;
    }

    private void Start()
    {
        playerTransform = player.GetComponent<Transform>();
        sprite = player.GetComponent<SpriteRenderer>();
    }
}

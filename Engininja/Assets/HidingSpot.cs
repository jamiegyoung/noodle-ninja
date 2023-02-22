using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HidingSpot : MonoBehaviour, Interactable
{

    public EnemyGenerator enemyGenerator;
    public GameObject player;
    private bool isHiding = false;

    public bool IsInteractable => !enemyGenerator.playerIsSeenAndBeingAttacked();
    private Rigidbody2D playerRigidbody;
    private SpriteRenderer playerSpriteRenderer;
    private ShadowCaster2D playerShadowCaster2D;


    private void Start()
    {
        playerRigidbody = player.GetComponent<Rigidbody2D>();
        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        playerShadowCaster2D = player.GetComponent<ShadowCaster2D>();
    }

    public void Interact()
    {
        Debug.Log("HIDING");
        if (isHiding)
        {
            isHiding = false;
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            player.gameObject.layer = LayerMask.NameToLayer("Player");
            playerSpriteRenderer.sortingLayerName = "Foreground";
            playerSpriteRenderer.sortingOrder = 0;
            playerShadowCaster2D.enabled = true;
            player.transform.position = gameObject.transform.position;
            player.transform.rotation = Quaternion.Euler(0, 0, 0);

        }
        else
        {
            isHiding = true;
            player.transform.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - .3f);
            player.transform.rotation = Quaternion.Euler(0, 0, -15);
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePosition;
            player.gameObject.layer = LayerMask.NameToLayer("PlayerHidden");
            playerSpriteRenderer.sortingLayerName = "Background";
            playerSpriteRenderer.sortingOrder = 1;
            playerShadowCaster2D.enabled = false;
        }
    }
}

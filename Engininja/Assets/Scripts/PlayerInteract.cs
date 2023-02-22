using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerInteract : MonoBehaviour
{

    public LayerMask interactableMask;
    public LayerMask enemyMask;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;
    private InputHandler inputHandler;
    private InteractionInformer interactionInformer;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        inputHandler = new InputHandler(GetComponent<PlayerInput>());
        rb = GetComponent<Rigidbody2D>();
        interactionInformer = transform.Find("InteractionInformer").GetComponent<InteractionInformer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, GetDirectionVector(), 1f, interactableMask + enemyMask);
        if (hit.collider == null)
        {
            interactionInformer.Hide();
            return;
        }
        Vector3 hitPos = hit.collider.transform.position;
        interactionInformer.Show(new Vector2(hitPos.x, hitPos.y + hit.collider.bounds.size.y + 0.3f), inputHandler.GetBindingDisplayString(InputHandlerActions.Attack));
        if (hit.collider != null)
        {
            Debug.Log("Interaction Possible with layer");
        }
        if (inputHandler.WasPressedThisFrame(InputHandlerActions.Attack))
        {
            // Only give forward momentum if enemy
            if (hit.collider.gameObject.layer == 10)
            {
                rb.velocity = new Vector2(rb.velocity.x + (hitPos.x - transform.position.x) * 50, rb.velocity.y);
            }
            hit.collider.GetComponent<Interactable>().Interact();
        }
    }

    private Vector2 GetDirectionVector()
    {
        if (sprite.flipX)
        {
            return Vector2.left;
        }
        else
        {
            return Vector2.right;
        }
    }
}

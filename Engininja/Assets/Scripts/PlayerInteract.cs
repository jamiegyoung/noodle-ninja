using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerInteract : MonoBehaviour
{

    public LayerMask interactableMask;
    public LayerMask enemyMask;
    public LayerMask blockingMask;
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
        RaycastHit2D hit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, GetDirectionVector(), 1f, interactableMask + enemyMask + blockingMask);
        if (hit.collider == null)
        {
            interactionInformer.Hide();
            return;
        }

        Vector3 hitPos = hit.collider.transform.position;
        // Hit blocking, not interactable
        Interactable interactable = hit.collider.GetComponent<Interactable>();
        if (interactable == null)
        {
            interactionInformer.Hide();
            return;
        }
        Vector2 interactionInformerPos = new(hitPos.x, hitPos.y + hit.collider.bounds.size.y + 0.3f);
        if (interactable.IsInteractable)
        {
            interactionInformer.Show(interactionInformerPos, inputHandler.GetBindingDisplayString(InputHandlerActions.Attack));
        }
        else
        {
            interactionInformer.ShowLock(interactionInformerPos);
        }

        if (inputHandler.WasPressedThisFrame(InputHandlerActions.Attack))
        {
            // Only give forward momentum if enemy
            if (hit.collider.gameObject.layer == 10)
            {
                rb.velocity = new Vector2(rb.velocity.x + (hitPos.x - transform.position.x) * 50, rb.velocity.y);
            }
            if (interactable.IsInteractable)
            {
                Debug.Log("Interacting");
                interactable.Interact(gameObject);
            }
            else
            {
                Debug.Log("Interaction blocked");
            }
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

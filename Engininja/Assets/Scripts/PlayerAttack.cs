using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{

    public LayerMask interactableMask;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;
    private InputHandler inputHandler;
    private InteractionInformer interactionInformer;
    private Rigidbody2D rb;

    public bool active = false;

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
    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, GetDirectionVector(), 1f, interactableMask);
        if (hit.collider == null)
        {
            interactionInformer.Hide();
            return;
        }
        Vector3 hitPos = hit.collider.transform.position;
        interactionInformer.Show(new Vector2(hitPos.x - 1.2f, hitPos.y + 2f), inputHandler.GetBindingDisplayString(InputHandlerActions.Attack));
        if (inputHandler.WasPressedThisFrame(InputHandlerActions.Attack))
        {
            Debug.Log("ATTACK!");
            hit.collider.GetComponent<Interactable>().Interact();
            rb.velocity = new Vector2(rb.velocity.x + (hitPos.x - transform.position.x) * 50, rb.velocity.y);
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

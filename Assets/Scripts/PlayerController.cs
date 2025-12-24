using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public Animator animator;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactRadius = 1.2f;
    public LayerMask interactLayerMask; 

    private Rigidbody2D rb;
    private Vector2 motionVector;
    public Vector2 lastMotionVector;
    public bool isMoving;

    public float interactCooldown = 0.2f;
    float nextInteract;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey) && Time.time >= nextInteract)
        {
            nextInteract = Time.time + interactCooldown;

        }
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        motionVector = new Vector2(horizontal, vertical).normalized;

        AnimateMovement(motionVector);

        isMoving = horizontal != 0 || vertical != 0;
        if (animator)
        {
            animator.SetBool("isMoving", isMoving);

            if (isMoving)
            {
                lastMotionVector = motionVector;
            }
            animator.SetFloat("lastHorizontal", lastMotionVector.x);
            animator.SetFloat("lastVertical", lastMotionVector.y);
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    void FixedUpdate()
    {
        Vector2 targetPos = rb.position + motionVector * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    void AnimateMovement(Vector2 direction)
    {
        if (animator != null)
        {
            bool moving = direction.sqrMagnitude > 0.0001f;
            animator.SetBool("isMoving", moving);
            if (moving)
            {
                animator.SetFloat("horizontal", direction.x);
                animator.SetFloat("vertical", direction.y);
            }
        }
    }

    void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactLayerMask);
        if (hits == null || hits.Length == 0) return;

        float bestSqr = float.MaxValue;
        IInteractable2D target = null;

        foreach (var h in hits)
        {
            if (h == null) continue;
            var interactable = h.GetComponent<IInteractable2D>();
            if (interactable == null) continue;

            float sqr = (h.transform.position - transform.position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                target = interactable;
            }
        }

        target?.Interact(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}

public interface IInteractable2D
{
    void Interact(GameObject interactor);
}

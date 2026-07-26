using System;
using UnityEngine;

public class PlatformMotion : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private float moveSpeed;
   
    [SerializeField] private bool isMoving = true;
    
    private int pointIndex;
    private bool isActive = true;
    
    private Collider2D platformCollider;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Vector2 currentTarget;

    private void Start() 
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        UpdateTarget();
    }
    
    private void FixedUpdate()
    {
        if (!isMoving) return;

        rb.MovePosition(Vector2.MoveTowards(rb.position, currentTarget, moveSpeed * Time.fixedDeltaTime));

        if (Vector2.Distance(rb.position, currentTarget) < 0.01f)
        {
            pointIndex = (pointIndex + 1) % points.Length;
            UpdateTarget();
        }
    }
    
    private void UpdateTarget()
    {
        Vector2 direction = ((Vector2)points[pointIndex].position - (Vector2)transform.position).normalized;

        // Flip(direction.x);

        float offset = Mathf.Abs(Vector2.Dot(platformCollider.bounds.extents, direction));

        currentTarget = (Vector2)points[pointIndex].position - direction * offset;
    }
    
    public void StartMove(bool isMoving)
    {
        this.isMoving = isMoving;
    }

    private void Flip(float direction)
    {
        if (direction > 0.1f) sprite.flipX = false;
        else if (direction < -0.1f) sprite.flipX = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!isActive) return;

        if (other.gameObject.CompareTag("Player"))
        {
            // other.transform.SetParent(transform, true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!isActive) return;
        
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.transform.parent == transform)
            {
                // other.transform.SetParent(null, true);
            }
        }
    }
}

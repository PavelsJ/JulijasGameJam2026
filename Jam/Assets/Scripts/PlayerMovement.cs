using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [Header("Ball")]
    [SerializeField] private float launchForce = 15f;
    [SerializeField] private float maxDragDistance = 4f;
    [SerializeField] private float stopVelocity = 0.3f;
    [SerializeField] private float wallImpactVelocity = 3f;
    [SerializeField] private float wallBounceBoost = 2f;

    public bool isBall;
    public bool isCharging;
    public bool isLaunched;
    
    private Vector2 lastCollisionNormal;
    
    private Vector2 dragStart;
    private Vector2 dragCurrent;
    
    [Header("Torque")]
    [SerializeField] private float randomTorqueMin = -0.2f; 
    [SerializeField] private float randomTorqueMax = 0.2f;  

    [Header("State")] 
    [SerializeField] private Collider2D[] playerCollider;
    
    [Header("Knockback")]
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float knockbackForce = 4;
    private bool isKnockbacked;
    
    [Header("Dependencies")]
    [SerializeField] private float groundCheckOffset = 0.5f;
    [SerializeField] private float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    
    private bool isGrounded;
    private bool wasGrounded;
    
    [Header("Softlock")]
    private float timeSinceLastJump = 0f;
    private float timeSinceLastMove = 0f;
    private const float maxLockTime = 10f;
    private Vector2 lastPosition;
    private bool isLocked = false;
    
    private Coroutine softlockRoutine;
    private float softlockCheckInterval = 2f;
    
    private float defaultGravity;
    
    private float footstepTimer;
    private float footstepInterval = 0.5f;

    private PlayerBase playerBase;
    
    private Coroutine knockbackRoutine;
    private Coroutine invulnerabilityRoutine;

    private Rigidbody2D rb;
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
        ManageBallCompounds(isBall);
    }

    public void Init(PlayerBase player)
    {
        playerBase = player;
        
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    public void TickUpdate()
    {
        UpdateGroundCheck();
        
        if (isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.X) 
                || Input.GetMouseButtonDown(1))
            {
                ToggleBallMode();
            }
        }
        
        if (isBall)
        {
            HandleBall();
            playerBase.Action.ShowAction();
        }
        else
        {
            HandleMovement();
            playerBase.Action.HideAction();
        }
        
        // HandleLanding();
    }

    public void TickFixedUpdate()
    {
        CheckAirTime();
        CheckIdleTime();
    }
    
    private void UpdateGroundCheck()
    {
        Vector2 checkPosition = (Vector2)transform.position + Vector2.down * groundCheckOffset;
        isGrounded = Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer);
    }
    
    private void CheckAirTime()
    {
        if (isLocked) return;
        
        if (isGrounded)
        {
            timeSinceLastJump = 0f;
        }
        else if (!playerBase.isDead)
        {
            timeSinceLastJump += Time.fixedDeltaTime;
            if (timeSinceLastJump >= maxLockTime)
            {
                SceneLoadManager.Instance.OpenReloadHint();
                isLocked = true;
                
                if (softlockRoutine != null)
                    StopCoroutine(softlockRoutine);

                softlockRoutine = StartCoroutine(SoftlockCheckRoutine());
            }
        }
    }
    
    private void CheckIdleTime()
    {
        if (isLocked) return;
        
        Vector2 currentPos = transform.position;
    
        if (Vector2.Distance(currentPos, lastPosition) < 0.01f)
        {
            timeSinceLastMove += Time.fixedDeltaTime;
            if (timeSinceLastMove >= maxLockTime)
            {
                SceneLoadManager.Instance.OpenReloadHint();
                isLocked = true;
                
                if (softlockRoutine != null)
                    StopCoroutine(softlockRoutine);

                softlockRoutine = StartCoroutine(SoftlockCheckRoutine());
            }
        }
        else
        {
            timeSinceLastMove = 0f;
        }

        lastPosition = currentPos;
    }
    
    private IEnumerator SoftlockCheckRoutine()
    {
        Vector2 startPos = transform.position;

        while (isLocked)
        {
            yield return new WaitForSeconds(softlockCheckInterval);

            if (Vector2.Distance(startPos, transform.position) >= 0.2f && isGrounded)
            {
                isLocked = false;
                SceneLoadManager.Instance.CloseReloadHint();
                yield break;
            }

            startPos = transform.position;
        }
    }
    
    private void ToggleBallMode()
    {
        isBall = !isBall;
        isLaunched = false;
        isCharging = false;
        
        ManageBallCompounds(isBall);
    }
    
    private void ExitBallMode()
    {
        if (!isBall) return;
        
        isBall = false;
        isLaunched = false;
        isCharging = false;
        
        ManageBallCompounds(isBall);
    }
    
    private void HandleBall()
    {
        if (camera == null) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!isGrounded)
            {
                playerBase.Action.WrongAction();
                return;
            }
            
            isCharging = true;
            
            dragStart = camera.ScreenToWorldPoint(Input.mousePosition);

            rb.linearVelocity = Vector2.zero;
        }

        if (isCharging)
        {
            dragCurrent = camera.ScreenToWorldPoint(Input.mousePosition);
            
            playerBase.Action.UpdateAim(transform.position,
                dragStart, dragCurrent, maxDragDistance);
            
            if (Input.GetMouseButtonUp(0))
            {
                Launch();
                playerBase.Action.HideAction();
            }
        }
        
        if (isLaunched && !isCharging && 
            rb.linearVelocity.magnitude < stopVelocity)
        {
            ExitBallMode();
        }
    }
    
    private void Launch()
    {
        wasGrounded = isGrounded;
        
        isCharging = false;
        isLaunched = true;
        
        footstepTimer = footstepInterval;
        playerBase.Visual.PlayJump(transform.position, true);

        Vector2 drag = dragStart - dragCurrent;
        drag = Vector2.ClampMagnitude(drag, maxDragDistance);
        rb.AddForce(drag * launchForce, ForceMode2D.Impulse);
        
        float randomTorque = Random.Range(randomTorqueMin, randomTorqueMax);
        rb.AddTorque(randomTorque, ForceMode2D.Impulse);
    }

    private void ManageBallCompounds(bool state)
    {
        if (playerCollider.Length == 0) return;
        
        rb.freezeRotation = !state;
        if (!state) transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        
        playerCollider[0].enabled = !state;
        playerCollider[1].enabled = state;
        
        playerBase.Visual.ChangeState(transform.position, state);
        
        Stop();
    }
    
    private void HandleMovement()
    {
        if(!isGrounded) return;
        float direction = Input.GetAxisRaw("Horizontal");
        
        Move(direction);
    }
    
    private void Move(float direction)
    {
        float targetSpeed = direction * moveSpeed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
        
        // float newSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, speedChange * Time.fixedDeltaTime);
        // rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);

        if (direction != 0)
        {
            playerBase.Visual.Flip(direction);
            playerBase.Visual.PlayMovement(direction);

            FootSteps();
        }
    }
    
    // private void HandleLanding()
    // {
    //     if (wasGrounded || !isGrounded) return;
    //     
    //     playerBase.Visual.PlayLand();
    //     playerBase.Visual.PlayJump(false);
    // }

    public void ApplyBoost(Vector2 hitDirection, float knockbackMultiplier = 1)
    {
        if (knockbackRoutine != null) StopCoroutine(knockbackRoutine);
        knockbackRoutine = StartCoroutine(ApplyKnockbackRoutine(hitDirection, knockbackMultiplier));
    }
    
    public void ApplyKnockback(Vector2 hitDirection, float knockbackMultiplier = 1)
    {
        if (playerBase.isInvulnerable) return;
        playerBase.Visual.PlayHit();
        
        if (invulnerabilityRoutine != null) StopCoroutine(invulnerabilityRoutine);
        invulnerabilityRoutine = StartCoroutine(InvulnerabilityRoutine(invulnerabilityTime));
        
        if (knockbackRoutine != null) StopCoroutine(knockbackRoutine);
        knockbackRoutine = StartCoroutine(ApplyKnockbackRoutine(hitDirection, knockbackMultiplier));
    }
    
    private IEnumerator ApplyKnockbackRoutine(Vector2 hitDirection, float knockbackMultiplier)
    {
        isKnockbacked = true;
        
        rb.AddForce(hitDirection.normalized * (knockbackForce * knockbackMultiplier), ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGrounded);
       
        isKnockbacked = false;
    }
    
    private IEnumerator InvulnerabilityRoutine(float duration)
    {
        playerBase.isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        playerBase.isInvulnerable = false;
    }
    
    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
        playerBase.Visual.PlayMovement(0);
    }
    
    private void FootSteps()
    {
        if (isGrounded)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                // MusicManager.instance.PlayRandomSound(MusicManager.SoundType.KnightFootsteps, Random.Range(0.9f, 1.1f));
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBall) return;

        if (rb.linearVelocity.magnitude < wallImpactVelocity) return;

        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal;

        playerBase.Visual.PlayLand(contact.point, normal);
        
        if (normal.y > 0.5f) return;
        float boost = rb.linearVelocity.magnitude * wallBounceBoost;
        rb.AddForce(normal * boost, ForceMode2D.Impulse);
    }
    
    private void OnDrawGizmos()
    {
        if (playerBase == null || !playerBase.isActive) return;
        if (!Application.isPlaying) return; 
        
        Vector2 checkPosition = (Vector2)transform.position + Vector2.down * groundCheckOffset;
        Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
    }
}

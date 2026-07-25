using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerVisual))]
public class PlayerBase : MonoBehaviour
{
    [Header("Knight States")]
    public bool isActive = false;
    public bool isDead = false;
    public bool isInvulnerable = false;
    
    public PlayerMovement Movement { get; private set; }
    public PlayerVisual Visual { get; private set; }
    
    private Coroutine playerCoroutine;

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
        Visual = GetComponent<PlayerVisual>();
        
        Movement.Init(this);
        Visual.Init(this);
    }
    
    private void Update()
    {
        if (isActive && !isDead)
            Movement.TickUpdate();
    }

    private void FixedUpdate()
    {
        if (isActive && !isDead)
            Movement.TickFixedUpdate();
    }
    
    public void Revive()
    {
        isDead = false;
        Visual.PlayDeath(isDead);

        ResetPlayer();
    }
    
    private void ResetPlayer()
    {
        Movement.Stop();
        
        //return to last position
        
        isActive = false;
    }
    
    public void HandleDamage(Vector2 hitDirection, float knockbackMultiplier = 1)
    {
        if (isInvulnerable || isDead) return;
        
        Movement.ApplyKnockback(hitDirection, knockbackMultiplier);
        
        HandleDefeat();
    }

    public void HandleBoost(Vector2 hitDirection, float knockbackMultiplier = 1)
    {
        if (isDead) return;
        
        Movement.ApplyBoost(hitDirection, knockbackMultiplier);
    }
    
    public void HandleDefeat()
    {
        isDead = true;
        isActive = false;
        
        Movement.Stop();
    }
    
    private void HandleFinish()
    {
        isActive = false;
        Movement.Stop();
    }
    
    
}

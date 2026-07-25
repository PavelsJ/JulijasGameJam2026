using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [Header("Player Trail")]
    public TrailRenderer trail;
    
    [Header("Player Particles")]
    public PlayerParticles particles;
    
    [Header("Player Material Effects")]
    public float flashTime = 1f;
    public AnimationCurve flashCurve;
    
    private PlayerBase playerBase;
    
    private SpriteRenderer sprite;
    private Animator animator;
    private Material material;

    public void Init(PlayerBase player)
    {
        playerBase = player;
        
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        material = new Material(sprite.material);
        sprite.material = material;
    }

    public void PlayHit()
    {
        particles.PlayHitParticles(transform.position);
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float currentFlashAmount = 0;
        float elapsedTime = 0;
        while (elapsedTime < flashTime)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1, flashCurve.Evaluate(elapsedTime), elapsedTime / flashTime);
            material.SetFloat("_HitEffectBlend", currentFlashAmount);
            yield return null;
        }
    }

    public void ChangeState(Vector2 position, bool state)
    {
        animator.SetBool("IsBall", state);
        sprite.color = state ? Color.green : Color.red;
        particles.PlaySwapParticles(position);
    }

    public void PlayDeath(bool isDead)
    {
        animator.SetBool("IsDead", isDead);
    }

    public void PlayJump(Vector2 position, bool jump)
    {
        animator.SetBool("Jump", jump);
        particles.PlayJumpParticles();
    }

    public void PlayLand(Vector2 position, Vector2 normal)
    {
        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
        particles.PlayLandingParticles(position, Quaternion.Euler(0, 0, angle));
    }
    
    public void PlayMovement(float direction)
    {
        animator.SetFloat("Move", direction);
    }
    
    public void Flip(float direction)
    {
        if (direction > 0.1f) sprite.flipX = false;
        else if (direction < -0.1f) sprite.flipX = true;
    }
}

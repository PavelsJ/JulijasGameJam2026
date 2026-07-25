using System.Collections;
using UnityEngine;

public class PadBase : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceStrength;
    [SerializeField] private float delay = 0.1f;
    [SerializeField] private ParticleSystem bounceEffect;
    
    private bool isActive = true;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;
        
        if (collision.CompareTag("Player"))
        {
            if (collision.isTrigger) return;
            PlayerBase player = collision.GetComponent< PlayerBase>();
            if (player != null && !player.isDead)
            {
                StartCoroutine(UsePad(player));
            }
        }
    }
    
    private IEnumerator UsePad(PlayerBase player)
    {
        isActive = false;
        bounceEffect.Play();

        yield return new WaitForSeconds(delay);

        if (player == null || player.isDead)
        {
            isActive = true;
            yield break;
        }

        // float directionX = Mathf.Sign(player.transform.position.x - transform.position.x);
        // Vector2 knockbackDirection = new Vector2(directionX, 2f).normalized;
        
        Vector2 knockbackDirection= transform.up;
        player.HandleBoost(knockbackDirection, bounceStrength);

        isActive = true;
    }
}

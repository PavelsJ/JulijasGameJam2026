using System.Collections;
using UnityEngine;

public class PadBase : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceStrength;
    [SerializeField] private float delay = 0.2f;
    [SerializeField] private ParticleSystem bounceEffect;
    
    [Header("Hit Feedback")]
    [SerializeField] private float moveUpDistance = 0.5f;
    [SerializeField] private float moveUpTime = 0.05f;
    [SerializeField] private float returnTime = 0.15f;
    
    private bool isActive = true;
    private Vector3 startPosition;
    
    private void Awake()
    {
        startPosition = transform.localPosition;
    }
    
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
        
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        float directionX = Mathf.Sign(rb.linearVelocity.x);
        
        if (Mathf.Abs(rb.linearVelocity.x) < 0.1f) 
            directionX = Random.Range(-0.5f,0.5f);
        
        Vector2 padDirection = transform.up;
        Vector2 knockbackDirection = (padDirection + (Vector2)transform.right * (directionX * 0.25f)).normalized;
        player.HandleBoost(knockbackDirection, bounceStrength);
        
        yield return StartCoroutine(HitFeedback());

        yield return new WaitForSeconds(delay);

        if (player == null || player.isDead)
        {
            isActive = true;
            yield break;
        }

        // float directionX = Mathf.Sign(player.transform.position.x - transform.position.x);
        // Vector2 knockbackDirection = new Vector2(directionX, 2f).normalized;
        
        // Vector2 knockbackDirection= transform.up;
        
        isActive = true;
    }
    
    private IEnumerator HitFeedback()
    {
        Vector3 upPosition = startPosition + transform.up * moveUpDistance;
        
        float timer = 0;

        while (timer < moveUpTime)
        {
            timer += Time.deltaTime; transform.localPosition = Vector3.Lerp(startPosition, upPosition, timer / moveUpTime);
            yield return null;
        }

        transform.localPosition = upPosition;
        timer = 0;

        while (timer < returnTime)
        {
            timer += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(upPosition, startPosition, timer / returnTime);

            yield return null;
        }

        transform.localPosition = startPosition;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SpikeBase : MonoBehaviour
{
    [Header("Bounce")]
    [SerializeField] private UnityEvent onSpike;
    [SerializeField] private UnityEvent offSpike;
    
    [SerializeField] private ParticleSystem zapEffect;
    
    [Header("Hit Feedback")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 0.08f;
    [SerializeField] private float scaleMultiplier = 1.1f;
    
    private bool isActive = true;
    
    private Vector3 defaultPosition;
    private Vector3 defaultScale;
    private Coroutine feedbackRoutine;
    
    private void Awake()
    {
        defaultScale = transform.localScale;
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
                float directionX = Mathf.Sign(player.transform.position.x - transform.position.x);
                Vector2 knockbackDirection = new Vector2(directionX, 2f).normalized;
                
                player.HandleDamage(knockbackDirection, 2);
                
                if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
                feedbackRoutine = StartCoroutine(HitFeedback());
            }
        }
    }
    
    private IEnumerator HitFeedback()
    {
        onSpike.Invoke();
        
        defaultPosition = transform.localPosition;
        transform.localScale = defaultScale * scaleMultiplier;

        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            Vector2 offset = Random.insideUnitCircle * shakeStrength;
            transform.localPosition = defaultPosition + (Vector3)offset;

            yield return null;
        }

        transform.localPosition = defaultPosition;
        transform.localScale = defaultScale;
        onSpike.Invoke();
    }
}

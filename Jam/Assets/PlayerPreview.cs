using System.Collections;
using UnityEngine;

public class PlayerPreview : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] private LineRenderer pullLine;
    [SerializeField] private LineRenderer directionLine;
    
    [SerializeField] private float flightDistance = 5f;
    
    [Header("Preview Settings")]
    public float shakeMagnitude = 0.2f;

    private SpriteRenderer previewRenderer;
    private Vector3 targetPosition;
    private Vector3 shakeOffset;
    
    private Coroutine shakeCoroutine;
    
    private void Awake()
    {
        previewRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ClearPreview();
    }
    
    public void UpdatePreview(Vector3 worldPos)
    {
        if (previewRenderer == null || !previewRenderer.enabled) return;
    
        targetPosition = worldPos;
        transform.position = targetPosition + shakeOffset;
    }
    
    public void UpdateAim(Vector3 playerPosition, Vector3 mouseStart, Vector3 mousePosition, float maxDistance)
    {
        Vector2 pullVector = mousePosition - mouseStart;
        float pullDistance = Mathf.Min(pullVector.magnitude, maxDistance);
        Vector2 pullDirection = pullVector.normalized;

        Vector3 pullEnd = mouseStart + (Vector3)(pullDirection * pullDistance);
        pullLine.enabled = true;

        pullLine.SetPosition(0, mouseStart);
        pullLine.SetPosition(1, pullEnd);

        Vector2 flightDirection = -pullDirection;
        Vector3 flightEnd = playerPosition + (Vector3)(flightDirection * flightDistance);

        directionLine.enabled = true;

        directionLine.SetPosition(0, playerPosition);
        directionLine.SetPosition(1, flightEnd);
    }


    public void ShowPreview(float newAlpha)
    {
        if (previewRenderer == null) return;
        previewRenderer.enabled = true;
        SetAlpha(newAlpha);
    }

    public void ClearPreview()
    {
        if (pullLine != null)
            pullLine.enabled = false;

        if (directionLine != null)
            directionLine.enabled = false;
        
        if (previewRenderer != null)
            previewRenderer.enabled = false;
    }

    public void HidePreview(float newAlpha)
    {
        if (previewRenderer == null) return;
        SetAlpha(newAlpha);
    }
    
    private void SetAlpha(float alpha)
    {
        if (previewRenderer == null) return;

        Color c = previewRenderer.color;
        c.a = alpha;
        previewRenderer.color = c;
    }

    public void ShakePreview(float duration)
    {
        if (previewRenderer == null || !previewRenderer.enabled)
            return;

        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeRoutine(duration));
    }

    private IEnumerator ShakeRoutine(float duration)
    {
        float elapsed = 0f;
    
        while (elapsed < duration)
        {
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeMagnitude,
                Random.Range(-1f, 1f) * shakeMagnitude,
                0f
            );
            elapsed += Time.deltaTime;
            yield return null;
        }
    
        shakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }
}

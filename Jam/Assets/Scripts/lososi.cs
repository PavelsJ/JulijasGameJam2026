using UnityEngine;

public class lososi : MonoBehaviour
{

    public float moveSpeed = 2f;
    public float amplitude = 1f; // на сколько пикселей/юнитов поднимается

    public float waveLength = 4f; // чем больше, тем плавнее и реже подъёмы


    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newX = transform.position.x + moveSpeed * Time.deltaTime;

        float distanceTraveled = newX - startPosition.x;
        float offsetY = amplitude * Mathf.Sin(2 * Mathf.PI * distanceTraveled / waveLength);
        transform.position = new Vector3(newX, startPosition.y + offsetY, transform.position.z);
    }
}
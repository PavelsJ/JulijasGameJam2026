using UnityEngine;

public class EntityMotion : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float amplitude = 0.1f;
    [SerializeField] private float cycleDuration = 4f;
    [SerializeField] private float rotateSpeed = 4f;

    private float startY;
    private float timer;
    
    public bool isMoving = true;
    public bool isRotating = false;
    
    private void Awake()
    {
        startY = transform.position.y;
        timer = Random.Range(0f, cycleDuration);
    }

    public void StartMove(bool isMoving)
    {
        this.isMoving = isMoving;
        isRotating = isMoving;
    }
    
    private void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        if (!isMoving) return;
        
        timer += Time.deltaTime;
        float t = (Mathf.Sin((timer / cycleDuration) * 2 * Mathf.PI) + 1f) / 2f; 
        float yOffset = Mathf.SmoothStep(-amplitude, amplitude, t);
        
        Vector3 pos = transform.position;
        pos.y = startY + yOffset;
        transform.position = pos;
    }

    private void Rotate()
    {
        if (!isRotating) return;

        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}

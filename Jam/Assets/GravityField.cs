using UnityEngine;

public class GravityField : MonoBehaviour
{
    [SerializeField] private Vector2 gravityDirection = Vector2.down;
    private bool isActive = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActive) return;
        
        if (collision.CompareTag("Player"))
        {
            if (collision.isTrigger) return;
            PlayerBase player = collision.GetComponent< PlayerBase>();
            if (player != null)
            {
                GameManager.Instance.ChangeGravity(gravityDirection);
            }
        }
    }
}

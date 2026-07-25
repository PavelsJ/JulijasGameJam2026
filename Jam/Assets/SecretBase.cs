using UnityEngine;

public class SecretBase : MonoBehaviour
{
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
                isActive = false;
                gameObject.SetActive(false);
            }
        }
    }
}

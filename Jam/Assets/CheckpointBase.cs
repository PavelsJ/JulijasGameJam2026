using UnityEngine;


public class CheckpointBase : MonoBehaviour
{
    [SerializeField] private Transform checkpoint;
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
                
                SetCheckpoint(checkpoint.position, Physics2D.gravity);
            }
        }
    }

    private void SetCheckpoint(Vector3 position, Vector2 gravity)
    {
        PlayerPrefs.SetFloat("CheckpointX", position.x);
        PlayerPrefs.SetFloat("CheckpointY", position.y);

        PlayerPrefs.SetFloat("GravityX", gravity.x);
        PlayerPrefs.SetFloat("GravityY", gravity.y);

        PlayerPrefs.Save();
    }
}
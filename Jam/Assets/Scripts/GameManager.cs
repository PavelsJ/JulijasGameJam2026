using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerBase playerBase;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        Time.timeScale = 1;
        Physics2D.gravity = new Vector2(0, -9.81f);
    }

    public void ChangeGravity(Vector2 direction)
    {
        Physics2D.gravity = direction.normalized * 9.81f;
        playerBase.Movement.ChangeGravity(direction);
        // CameraManager.instance.RotateToGravity(-direction);
    }
}

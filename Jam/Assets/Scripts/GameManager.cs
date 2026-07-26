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

        LoadSafe();
    }

    private void LoadSafe()
    {
        if (!PlayerPrefs.HasKey("CheckpointX")) return;

        Vector3 checkpoint = new Vector3(
            PlayerPrefs.GetFloat("CheckpointX"),
            PlayerPrefs.GetFloat("CheckpointY"),
            PlayerPrefs.GetFloat("CheckpointZ")
        );

        Vector2 gravity = new Vector2(
            PlayerPrefs.GetFloat("GravityX"),
            PlayerPrefs.GetFloat("GravityY")
        );

        playerBase.transform.position = checkpoint;
        ChangeGravity(gravity);
    }

    public void ChangeGravity(Vector2 direction)
    {
        Physics2D.gravity = direction.normalized * 9.81f;
        playerBase.Movement.ChangeGravity(direction);
        // CameraManager.instance.RotateToGravity(-direction);
    }
}

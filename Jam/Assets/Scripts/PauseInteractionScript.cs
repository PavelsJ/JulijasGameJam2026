using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseInteractionScript : MonoBehaviour
{
    [SerializeField] private GameObject pause;
    
    void Start()
    {
        pause.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        pause.SetActive(!pause.activeSelf);
        SetTime(pause.activeSelf ? 1 : 0);
    }

    public void OpenPause()
    {
        pause.SetActive(true);
        SetTime(0);
    }

    public void ClosePause()
    {
        pause.SetActive(false);
        SetTime(1);
    }

    private void SetTime(float mult)
    {
        Time.timeScale = mult;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    
    [SerializeField] private Transform virtualCamera;
    [SerializeField] private float rotationSmoothTime = 0.25f;

    private float rotationVelocity;
    private float targetAngle;
    
    private Vector3 originalPosition;  

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // private void LateUpdate()
    // {
    //     float angle = Mathf.SmoothDampAngle(virtualCamera.eulerAngles.z, targetAngle, ref rotationVelocity, rotationSmoothTime);
    //     virtualCamera.rotation = Quaternion.Euler(0, 0, angle);
    // }
    
    public void RotateToGravity(Vector2 gravity)
    {
        targetAngle = Mathf.Atan2(-gravity.x, gravity.y) * Mathf.Rad2Deg;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(ShakeEffect(duration, magnitude));
    }
    
    private IEnumerator ShakeEffect(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.position = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }
}

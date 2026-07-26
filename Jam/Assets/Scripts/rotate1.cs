using UnityEngine;

public class rotate : MonoBehaviour
{

    public float speed = 35f;
    void Update()
    {

        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }
}

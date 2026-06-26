using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed = 10f;
    public Vector3 rotationAxis = Vector3.up;

    void FixedUpdate()
    {
        transform.Rotate(rotationAxis, speed * Time.deltaTime);
    }
}

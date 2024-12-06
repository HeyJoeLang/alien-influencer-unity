using UnityEngine;
public class Billboard : MonoBehaviour
{
    Transform camTransform;
    Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
        camTransform = Camera.main.transform;
    }

    void FixedUpdate()
    {
        transform.LookAt(camTransform.position, Vector3.up);
    }
}

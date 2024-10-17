using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class UfoMovement : MonoBehaviour
{
    public float moveSpeed = 25.0f;
    public float acceleration = 10.0f;
    public float deceleration = 20.0f;
    public float rotationSpeed = 50.0f;
    public float tiltAmount = 0f;
    public Terrain terrain;
    private Rigidbody rb;
    public float targetHeight = 8f;
    public float heightLerpSpeed = 3.0f;

    private float minXPosition;
    private float maxXPosition;
    private float minZPosition;
    private float maxZPosition;
    private Vector3 terrainCenter;

    private float currentSpeed = 0.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        InitializeUfoPosition();
    }

    private void InitializeUfoPosition()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain not assigned in the UfoMovement script!");
            return;
        }

        /* *
        * Getting bounds of the terrain to spawn UFO at center of the terrain
        * */

        TerrainData terrainData = terrain.terrainData;
        minXPosition = terrain.transform.position.x;
        maxXPosition = terrain.transform.position.x + terrainData.size.x;
        minZPosition = terrain.transform.position.z;
        maxZPosition = terrain.transform.position.z + terrainData.size.z;
        terrainCenter = new Vector3((minXPosition + maxXPosition) / 2, 0, (minZPosition + maxZPosition) / 2);

        float terrainHeight = terrain.SampleHeight(terrainCenter) + terrain.transform.position.y;
        rb.position = new Vector3(terrainCenter.x, terrainHeight + targetHeight, terrainCenter.z);
    }

    private void FixedUpdate()
    {
        if (terrain == null)
            return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float rotation = horizontalInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        if (Mathf.Abs(verticalInput) > 0)
        {
            currentSpeed += verticalInput * acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, -moveSpeed, moveSpeed);
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0);
            }
        }

        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        float clampedX = Mathf.Clamp(rb.position.x, minXPosition, maxXPosition);
        float clampedZ = Mathf.Clamp(rb.position.z, minZPosition, maxZPosition);

        /* *
        * Lerp UFO height to terrain height
        * */

        float terrainHeight = terrain.SampleHeight(new Vector3(clampedX, 0, clampedZ)) + terrain.transform.position.y;

        float currentHeight = rb.position.y;
        float desiredHeight = Mathf.Lerp(currentHeight, terrainHeight + targetHeight, heightLerpSpeed * Time.fixedDeltaTime);

        Vector3 newPosition = new Vector3(clampedX, desiredHeight, clampedZ);
        rb.MovePosition(newPosition);
    }
}

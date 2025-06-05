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
    public float targetHeight = 10f;
    public float minHeight = 6f;
    public float maxHeight = 15f;
    public float heightChangeSpeed = 5f;
    public float heightLerpSpeed = 3.0f;

    private float minXPosition;
    private float maxXPosition;
    private float minZPosition;
    private float maxZPosition;
    private Vector3 terrainCenter;

    //private float currentSpeed = 0.0f;

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
        //Subjective to change
        minXPosition = -330f;
        maxXPosition = 165f;
        minZPosition = -280f;
        maxZPosition = 15f;
    }

    private void FixedUpdate()
    {
        if (terrain == null)
            return;
        // Handling rotation (horizontal input) and forward movement (vertical input)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float currentHeight = transform.position.y;
        float clampedX = Mathf.Clamp(transform.position.x, minXPosition, maxXPosition);
        float clampedZ = Mathf.Clamp(transform.position.z, minZPosition, maxZPosition);
        float terrainHeight = terrain.SampleHeight(new Vector3(clampedX, 0, clampedZ)) + terrain.transform.position.y;
        // Rotate UFO around Y axis based on horizontal input
        transform.Rotate(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);

        float desiredHeight = Mathf.Lerp(currentHeight, terrainHeight + targetHeight, heightLerpSpeed * Time.fixedDeltaTime);

        // Move UFO forward/backward based on vertical input
        Vector3 forwardMovement = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
        Vector3 sideMovement = transform.right * horizontalInput * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + forwardMovement + sideMovement;

        // Clamp movement within terrain bounds
        newPosition.x = Mathf.Clamp(newPosition.x, minXPosition, maxXPosition);
        newPosition.z = Mathf.Clamp(newPosition.z, minZPosition, maxZPosition);
        newPosition.y = desiredHeight;

        // Apply the new position (only updating X and Z for now)
        transform.position = newPosition;
    }
}

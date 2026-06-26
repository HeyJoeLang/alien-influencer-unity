using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class UfoMovement : MonoBehaviour
{
    public float normalizedVelocity;
    public float lerpedVelocity;
    [Range(0.5f,3.0f)]
    public float FModAcceleration = 1.5f;
    public float moveSpeed = 25.0f;
    public float rotationSpeed = 50.0f;
    public Terrain terrain;
    private Rigidbody rb;
    public float targetHeight = 8f;
    public float heightLerpSpeed = 3.0f;

    private AudioHandle ufoEngineHandle = AudioHandle.Invalid;

    private float minXPosition;
    private float maxXPosition;
    private float minZPosition;
    private float maxZPosition;
    private Vector3 terrainCenter;
    private Vector3 lastPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        InitializeUfoPosition();

        ufoEngineHandle = AudioManager.Instance.PlayLoop(AudioSoundIds.SoundDesign.Vehicles.UfoHover, transform);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopLoop(ref ufoEngineHandle, 0.2f);
    }

    private void InitializeUfoPosition()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain not assigned in the UfoMovement script!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        minXPosition = -330f;
        maxXPosition = 165f;
        minZPosition = -280f;
        maxZPosition = 15f;
    }

    private void FixedUpdate()
    {
        if (terrain == null)
            return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float currentHeight = transform.position.y;
        float clampedX = Mathf.Clamp(transform.position.x, minXPosition, maxXPosition);
        float clampedZ = Mathf.Clamp(transform.position.z, minZPosition, maxZPosition);
        float terrainHeight = terrain.SampleHeight(new Vector3(clampedX, 0, clampedZ)) + terrain.transform.position.y;
        transform.Rotate(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);

        float desiredHeight = Mathf.Lerp(currentHeight, terrainHeight + targetHeight, heightLerpSpeed * Time.fixedDeltaTime);

        Vector3 forwardMovement = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
        Vector3 sideMovement = transform.right * horizontalInput * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + forwardMovement + sideMovement;

        newPosition.x = Mathf.Clamp(newPosition.x, minXPosition, maxXPosition);
        newPosition.z = Mathf.Clamp(newPosition.z, minZPosition, maxZPosition);
        newPosition.y = desiredHeight;

        transform.position = newPosition;

        Vector3 velocity = (newPosition - lastPosition) / Time.fixedDeltaTime;
        normalizedVelocity = moveSpeed <= 0f ? 0f : Mathf.Clamp01(velocity.magnitude / moveSpeed);
        lerpedVelocity = Mathf.Lerp(lerpedVelocity, normalizedVelocity, Time.fixedDeltaTime * FModAcceleration);
        lerpedVelocity = Mathf.Clamp01(lerpedVelocity);
        float enginePitch = Mathf.Lerp(0.75f, 1.25f, lerpedVelocity);
        AudioManager.Instance.SetLoopPitch(ufoEngineHandle, enginePitch);
        AudioManager.Instance.SetLoopVolume(ufoEngineHandle, Mathf.Lerp(0.35f, 1f, lerpedVelocity));
        lastPosition = newPosition;
    }
}

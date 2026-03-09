using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODAudioExample : MonoBehaviour
{
    [Header("FMOD Event")]
    [SerializeField] private EventReference eventReference;   // Drag your FMOD event in the Inspector

    [Header("Parameter Settings")]
    [SerializeField] private string parameterName = "Intensity"; // Must match the parameter name in FMOD Studio
    [Range(0f, 1f)]
    public float parameterValue = 0f;
    
    private EventInstance _eventInstance;
    private bool          _isPlaying = false;

    private void Start()
    {
        _eventInstance = RuntimeManager.CreateInstance(eventReference);
        RuntimeManager.AttachInstanceToGameObject(_eventInstance, transform, GetComponent<Rigidbody>());
        _eventInstance.start();
    }

    private void Update()
    {
        FMOD.RESULT result = _eventInstance.setParameterByName(parameterName, parameterValue);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning($"[FMODLoopedSound] Could not set parameter '{parameterName}': {result}");
        }
    }

    private void OnDestroy()
    {
        StopAndRelease();
    }

    private void OnDisable()
    {
        StopAndRelease();
    }
    public void StopAndRelease()
    {
        _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _eventInstance.release();
    }
}

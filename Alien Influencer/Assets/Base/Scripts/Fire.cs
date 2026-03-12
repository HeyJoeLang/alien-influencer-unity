using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class Fire : MonoBehaviour
{
    [SerializeField] private EventReference eventFire;
    private EventInstance fireEventInstance;
    void Start()
    {
        fireEventInstance = RuntimeManager.CreateInstance(eventFire);
    }
    public void StopFire()
    {
        fireEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void StartFire()
    {
        RuntimeManager.AttachInstanceToGameObject(fireEventInstance, transform);
        fireEventInstance.start();
        Debug.Log("Starting fire");
        fireEventInstance.getPlaybackState(out var playbackState);
        Debug.Log("Playback state: " + playbackState);
    }
}

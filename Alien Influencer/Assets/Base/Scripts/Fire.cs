using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class Fire : MonoBehaviour
{
    [SerializeField] private EventReference eventFire;
    private EventInstance fireEventInstance;
    bool isActive = false;
    float distance = 0f;
    void Start()
    {
        fireEventInstance = RuntimeManager.CreateInstance(eventFire);
        isActive = false;
    }
    public void StopFire()
    {
        fireEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isActive = false;
    }
    public void StartFire()
    {
        RuntimeManager.AttachInstanceToGameObject(fireEventInstance, transform);
        fireEventInstance.setVolume(1.0f);
        fireEventInstance.start();
        isActive = true;
    }
    public void Update()
    {
        if (isActive)
        {
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            if(distance < 10f)
            {
                fireEventInstance.setParameterByName("Distance", distance);
            }
        }
        else
        {
            fireEventInstance.setVolume(0.0f);
        }
    }
}

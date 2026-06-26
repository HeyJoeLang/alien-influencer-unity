using UnityEngine;

public class Fire : MonoBehaviour
{
    private AudioHandle fireHandle;
    bool isActive = false;
    float distance = 0f;
    
    public void StopFire()
    {
        AudioManager.Instance.StopLoop(ref fireHandle, 0.2f);
        isActive = false;
    }
    public void StartFire()
    {
        fireHandle = AudioManager.Instance.PlayLoop(AudioSoundIds.SoundDesign.Destruction.Fire, transform);
        isActive = true;
    }
    public void Update()
    {
        if (!isActive || Camera.main == null)
        {
            return;
        }

        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        if(distance < 10f)
        {
            AudioManager.Instance.SetLoopDistanceAttenuation(fireHandle, distance, 10f);
        }
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopLoop(ref fireHandle, 0f);
    }
}

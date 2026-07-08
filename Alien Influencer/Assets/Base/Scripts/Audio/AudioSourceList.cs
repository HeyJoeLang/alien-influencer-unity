using UnityEngine;

public class AudioSourceList : MonoBehaviour
{
    public AudioSource Source;
    public AudioClip[] Clips;

    public void Play()
    {
        int randomIndex = Random.Range(0, Clips.Length);
        Source.clip = Clips[randomIndex];
        Source.Play();
    }

    public void Stop()
    {
        Source.Stop();
    }
}

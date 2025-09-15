using UnityEngine;
using UnityEngine.Video;

public class InputControlsVideo : MonoBehaviour
{
    public VideoClip[] movieClips;
    public VideoPlayer videoPlayer;
    private int index = 0;

    public void PlayNextMovie()
    {
        if (!movieClips[index])
        {
            Debug.LogError("No movie clip found at index " + index);
        }
        videoPlayer.clip = movieClips[index];
        videoPlayer.Play();
        index++;
    }
}

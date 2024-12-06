using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class MusicSelector : MonoBehaviour
{
    [System.Serializable]
    public class Music
    {
        public string name;
        public AudioClip clip;
    }
    public Music[] musics;
    int index = 0;
    AudioSource audioSource;
    public TMP_Text songText;

    public void PlayNextMusic()
    {
        index++;
        if (index >= musics.Length)
        {
            index = 0;
        }
        songText.text = musics[index].name;
        audioSource.clip = musics[index].clip;
        audioSource.Play();
    }
    public void PlayPreviousMusic()
    {
        index--;
        if (index < 0)
        {
            index = musics.Length - 1;
        }
        songText.text = musics[index].name;
        audioSource.clip = musics[index].clip;
        audioSource.Play();
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        index = 0;
        songText.text = musics[index].name;
        audioSource.clip = musics[index].clip;
        audioSource.Play();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayPreviousMusic();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayNextMusic();
        }
    }
}

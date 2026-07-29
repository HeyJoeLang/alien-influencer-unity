using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct AudioItem
{
    public AudioSource Source;
    public void Play()
    {
        Source.Play();
    }
    public void Stop()
    {
        Source.Stop();
    }
}

[System.Serializable]
public struct AudioList
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

public class AudioManager : Singleton<AudioManager>
{
    
    [SerializeField]
    private AudioMixer _mixer;
    
    [Header(("SetUp"))]
    [SerializeField]
    private AudioItem _ufoLaser;
    [SerializeField]
    private AudioItem _ufoMegaLaser;
    [SerializeField]
    private AudioList _ufoMissileLaunch;
    [SerializeField]
    private AudioList _ufoForceFieldActivation;
    [SerializeField]
    private AudioList _ufoForceFieldBounce; 
    [SerializeField]
    private AudioItem _ufoHoverStill;
    [SerializeField]
    private AudioItem _ufoHoverMoving;
    [SerializeField]
    private AudioItem _collectMegaLaserCharge;
    [SerializeField]
    private AudioItem _collectMissileCharge;
    [SerializeField]
    private AudioList _ufoDamage;
    [SerializeField]
    private AudioItem _resetScore;
    
    [Header(("ToDo"))]
    //
    [SerializeField]
    private AudioList _deflectedMissileExplosion;
    [SerializeField]
    private AudioItem _buildingFire;
    [SerializeField]
    private AudioList _alienCheers;
    
    #region PlayStopFunctions

    public void PlayUfoLaser()
    {
        _ufoLaser.Play();
    }

    public void StopUfoLaser()
    {
        _ufoLaser.Stop();
    }

    public void PlayUfoMegaLaser()
    {
        _ufoMegaLaser.Play();
    }

    public void StopUfoMegaLaser()
    {
        _ufoMegaLaser.Stop();
    }
    public void PlayUfoMissileLaunch()
    {
        _ufoMissileLaunch.Play();
    }
    public void PlayUfoForceFieldActivation()
    {
        _ufoForceFieldActivation.Play();
    }
    
    public void PlayUfoForceFieldBounce()
    {
        _ufoForceFieldBounce.Play();
    }
    public void PlayCollectMissileCharge()
    {
        _collectMissileCharge.Play();
    }
    public void PlayCollectMegaLaserCharge()
    {
        _collectMegaLaserCharge.Play();
    }

    public void SetUFOAduioSpeed(float speed)
    {
        _ufoHoverStill.Source.volume = 1 - speed;
        _ufoHoverMoving.Source.volume = speed;
    }

    public void PlayUfoDamage()
    {
        _ufoDamage.Play();
    }

    public void PlayResetScore()
    {
        _resetScore.Play();
    }
    #endregion
}

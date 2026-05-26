using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AlienAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    [SerializeField] private EventReference eventCheer;
    [SerializeField] private EventReference eventGrumble;
    private EventInstance cheerEventInstance;   
    private EventInstance grumbleEventInstance;   
    
    [SerializeField] private string[] celebrationClips = {
        "cheer1",
        "cheer2", 
        "cheer3", 
        "cheer4", 
        "cheer5"
    };
    
    [SerializeField] private string[] hitClips = {
        "Hit1",
        "Hit2",
        "Hit3"
    };
    
    private int celebrationIterator = 0;
    private int hitIterator = 0;

    private float lastMassDestructionTime = -1f;
    private float lastMinorDestructionTime = -1f;
    private const float destructionCooldown = 1f;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        
        
        cheerEventInstance = RuntimeManager.CreateInstance(eventCheer);
        grumbleEventInstance = RuntimeManager.CreateInstance(eventGrumble);
        RuntimeManager.AttachInstanceToGameObject(cheerEventInstance, transform);
        RuntimeManager.AttachInstanceToGameObject(grumbleEventInstance, transform);

        BuildingManager.OnMassDestruction += HandleMassDestruction;
        BuildingManager.OnMinorDestruction += HandleMinorDestruction;
    }

    private void OnDestroy()
    {
        BuildingManager.OnMassDestruction -= HandleMassDestruction;
        BuildingManager.OnMinorDestruction -= HandleMinorDestruction;
    }

    private void HandleMassDestruction(int count)
    {
        if (Time.time - lastMassDestructionTime >= destructionCooldown)
        {
            lastMassDestructionTime = Time.time;
            StartCoroutine(StallCheers());
            Debug.Log($"AlienAnimationManager: Mass Destruction detected! {count} buildings destroyed rapidly");
            
        }
    }

    IEnumerator StallCheers()
    {
        yield return new WaitForSeconds(.75f);
        
        cheerEventInstance.getPlaybackState(out var playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING)
        {
            cheerEventInstance.start();
        }
    }

    private void HandleMinorDestruction(int count)
    {
        if (Time.time - lastMinorDestructionTime >= destructionCooldown)
        {
            lastMinorDestructionTime = Time.time;
            Debug.Log($"AlienAnimationManager: Minor Destruction detected. {count} buildings destroyed");
        }
    }
    public void AlienCelebrate()
    {
        if (celebrationClips.Length > 0 && animator != null)
        {
            Debug.Log($"AlienAnimation: {celebrationClips[celebrationIterator]}");
            animator.SetTrigger(celebrationClips[celebrationIterator]);
            celebrationIterator = (celebrationIterator + 1) % celebrationClips.Length;
        }
    }
    
    public void AlienHit()
    {
        if (hitClips.Length > 0 && animator != null)
        {
            animator.SetTrigger(hitClips[hitIterator]);
            hitIterator = (hitIterator + 1) % hitClips.Length;
        }
    }
}
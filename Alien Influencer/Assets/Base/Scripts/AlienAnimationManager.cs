using UnityEngine;

public class AlienAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
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
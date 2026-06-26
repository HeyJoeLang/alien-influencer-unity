using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSoundRegistry", menuName = "Audio/Sound Registry")]
public class AudioSoundRegistry : ScriptableObject
{
    public enum OverflowStrategy
    {
        DropNew,
        StealOldest,
        StealQuietest
    }

    [System.Serializable]
    public class SoundEntry
    {
        public string soundId;
        [Tooltip("When empty, playback is skipped with a TODO warning.")]
        public AudioClip[] clips;
        public bool is2D;
        public bool loop;
        [Range(0, 128)] public int priority = 64;
        [Min(1)] public int maxConcurrent = 8;
        public OverflowStrategy overflowStrategy = OverflowStrategy.DropNew;
        public float minDistance = 1f;
        public float maxDistance = 50f;
    }

    public List<SoundEntry> entries = new List<SoundEntry>();

    private Dictionary<string, SoundEntry> lookup;

    public void BuildLookup()
    {
        lookup = new Dictionary<string, SoundEntry>();
        foreach (SoundEntry entry in entries)
        {
            if (string.IsNullOrEmpty(entry.soundId))
            {
                continue;
            }

            lookup[entry.soundId] = entry;
        }
    }

    public bool TryGetEntry(string soundId, out SoundEntry entry)
    {
        if (lookup == null)
        {
            BuildLookup();
        }

        return lookup.TryGetValue(soundId, out entry);
    }

    public AudioClip PickClip(SoundEntry entry)
    {
        if (entry.clips == null || entry.clips.Length == 0)
        {
            return null;
        }

        if (entry.clips.Length == 1)
        {
            return entry.clips[0];
        }

        return entry.clips[Random.Range(0, entry.clips.Length)];
    }
}

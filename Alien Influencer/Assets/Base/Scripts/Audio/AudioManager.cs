using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AudioHandle
{
    internal int SourceIndex;
    internal int Generation;

    public static readonly AudioHandle Invalid = new AudioHandle { SourceIndex = -1, Generation = -1 };

    public bool IsValid => SourceIndex >= 0;
}

public class AudioManager : Singleton<AudioManager>
{
    private class PooledVoice
    {
        public AudioSource Source;
        public bool InUse;
        public bool IsLoop;
        public string SoundId;
        public Transform Follow;
        public float StartTime;
        public float BaseVolume = 1f;
        public int Priority;
        public int Generation;
        public Coroutine ReturnCoroutine;
    }

    [Header("Registry")]
    [SerializeField] private AudioSoundRegistry registry;

    [Header("Pool")]
    [SerializeField] private int poolSize = 32;

    [Header("Music")]
    [SerializeField] private float musicVolume = 0.7f;

    [Header("Debug")]
    [SerializeField] private bool debugLogging = true;
    [Tooltip("Also log successful plays (can be noisy during loops).")]
    [SerializeField] private bool debugLogSuccessfulPlays;

    private readonly List<PooledVoice> pool = new List<PooledVoice>();
    private readonly Dictionary<string, List<PooledVoice>> activeVoicesBySound = new Dictionary<string, List<PooledVoice>>();
    private readonly HashSet<string> loggedMissingSounds = new HashSet<string>();

    private AudioSource musicSource;
    private string currentMusicId;
    private Coroutine musicFadeCoroutine;

    private Transform poolRoot;

    protected override void Awake()
    {
        base.Awake();
        if (!IsSingletonInstance)
        {
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (poolRoot != null)
        {
            return;
        }

        if (registry == null)
        {
            registry = Resources.Load<AudioSoundRegistry>("AudioSoundRegistry");
        }

        if (registry != null)
        {
            registry.BuildLookup();
            LogInitSummary();
        }
        else
        {
            Debug.LogError("[AudioManager] Failed to load AudioSoundRegistry from Resources/AudioSoundRegistry.");
        }

        poolRoot = new GameObject("SFXPool").transform;
        poolRoot.SetParent(transform);

        for (int i = 0; i < poolSize; i++)
        {
            var sourceGo = new GameObject($"Voice_{i}");
            sourceGo.transform.SetParent(poolRoot);
            var source = sourceGo.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            pool.Add(new PooledVoice
            {
                Source = source,
                Generation = 0
            });
        }
        LogDebug($"Initialized: poolSize={poolSize}, listenerCount={CountAudioListeners()}, mute={AudioListener.pause}");
    }

    private void LateUpdate()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            PooledVoice voice = pool[i];
            if (!voice.InUse || voice.Follow == null)
            {
                continue;
            }

            voice.Source.transform.position = voice.Follow.position;
        }
    }

    public void Play(string soundId, Vector3 position, float volume = 1f)
    {
        if (!TryGetClip(soundId, out AudioSoundRegistry.SoundEntry entry, out AudioClip clip))
        {
            return;
        }

        if (!EnsureCanHearAudio(soundId))
        {
            return;
        }

        if (!TryAcquireVoice(soundId, entry, position, null, false, volume, out PooledVoice voice))
        {
            LogPlayFailure(soundId, "no free voice in pool (all slots busy)");
            return;
        }

        ConfigureSource(voice.Source, entry, clip, volume, false);
        voice.Source.transform.position = position;
        voice.Source.Play();

        voice.ReturnCoroutine = StartCoroutine(ReturnWhenFinished(voice));
        LogPlaySuccess(soundId, clip, volume, position, entry.is2D ? "2D" : "3D");
    }

    public void Play(string soundId, Transform follow, float volume = 1f)
    {
        if (follow == null)
        {
            Play(soundId, Vector3.zero, volume);
            return;
        }

        Play(soundId, follow.position, volume);

        PooledVoice voice = GetLastStartedVoice(soundId);
        if (voice != null)
        {
            voice.Follow = follow;
        }
    }

    public void Play2D(string soundId, float volume = 1f)
    {
        if (!TryGetClip(soundId, out AudioSoundRegistry.SoundEntry entry, out AudioClip clip))
        {
            return;
        }

        if (!EnsureCanHearAudio(soundId))
        {
            return;
        }

        if (!TryAcquireVoice(soundId, entry, Vector3.zero, null, false, volume, out PooledVoice voice))
        {
            LogPlayFailure(soundId, "no free voice in pool (all slots busy)");
            return;
        }

        ConfigureSource(voice.Source, entry, clip, volume, true);
        voice.Source.spatialBlend = 0f;
        voice.Source.Play();

        voice.ReturnCoroutine = StartCoroutine(ReturnWhenFinished(voice));
        LogPlaySuccess(soundId, clip, volume, Vector3.zero, "2D");
    }

    public AudioHandle PlayLoop(string soundId, Transform follow, float volume = 1f)
    {
        if (follow == null)
        {
            Debug.LogWarning($"AudioManager.PlayLoop requires a follow transform for {soundId}");
            return AudioHandle.Invalid;
        }

        if (!TryGetClip(soundId, out AudioSoundRegistry.SoundEntry entry, out AudioClip clip))
        {
            return AudioHandle.Invalid;
        }

        if (!EnsureCanHearAudio(soundId))
        {
            return AudioHandle.Invalid;
        }

        if (!TryAcquireVoice(soundId, entry, follow.position, follow, true, volume, out PooledVoice voice))
        {
            LogPlayFailure(soundId, "no free voice in pool (all slots busy)");
            return AudioHandle.Invalid;
        }

        ConfigureSource(voice.Source, entry, clip, volume, entry.is2D);
        voice.Source.loop = true;
        voice.Source.transform.position = follow.position;
        voice.Source.Play();

        LogPlaySuccess(soundId, clip, volume, follow.position, entry.is2D ? "2D loop" : "3D loop");
        return new AudioHandle { SourceIndex = pool.IndexOf(voice), Generation = voice.Generation };
    }

    public void StopLoop(ref AudioHandle handle, float fadeOut = 0.2f)
    {
        if (!TryGetVoice(handle, out PooledVoice voice))
        {
            handle = AudioHandle.Invalid;
            return;
        }

        if (fadeOut <= 0f)
        {
            ReleaseVoice(voice);
        }
        else
        {
            StartCoroutine(FadeOutAndRelease(voice, fadeOut));
        }

        handle = AudioHandle.Invalid;
    }

    public bool IsLoopPlaying(AudioHandle handle)
    {
        return TryGetVoice(handle, out PooledVoice voice) && voice.Source.isPlaying;
    }

    public void SetLoopVolume(AudioHandle handle, float volume)
    {
        if (!TryGetVoice(handle, out PooledVoice voice))
        {
            return;
        }

        voice.BaseVolume = volume;
        voice.Source.volume = volume;
    }

    public void SetLoopPitch(AudioHandle handle, float pitch)
    {
        if (!TryGetVoice(handle, out PooledVoice voice))
        {
            return;
        }

        voice.Source.pitch = Mathf.Clamp(pitch, 0.1f, 3f);
    }

    public void SetLoopDistanceAttenuation(AudioHandle handle, float distance, float maxDistance = 10f)
    {
        if (!TryGetVoice(handle, out PooledVoice voice))
        {
            return;
        }

        float t = maxDistance <= 0f ? 0f : Mathf.Clamp01(1f - distance / maxDistance);
        voice.Source.volume = voice.BaseVolume * t;
    }

    public void StopMusic(float fadeOut = 1f)
    {
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        if (fadeOut <= 0f)
        {
            musicSource.Stop();
            currentMusicId = null;
            return;
        }

        musicFadeCoroutine = StartCoroutine(FadeMusicOut(fadeOut));
    }

    internal bool IsHandleValid(AudioHandle handle)
    {
        return TryGetVoice(handle, out _);
    }

    private bool TryGetClip(string soundId, out AudioSoundRegistry.SoundEntry entry, out AudioClip clip)
    {
        entry = null;
        clip = null;

        if (registry == null)
        {
            Debug.LogError("[AudioManager] No AudioSoundRegistry assigned or loaded from Resources.");
            return false;
        }

        if (!registry.TryGetEntry(soundId, out entry))
        {
            LogMissingSound(soundId, "not registered — run Audio > Build Sound Registry From Non-FMOD or check AudioSoundIds");
            return false;
        }

        clip = registry.PickClip(entry);
        if (clip == null)
        {
            int clipCount = entry.clips?.Length ?? 0;
            LogMissingSound(soundId, $"no clips assigned ({clipCount} slots) — add wav under Assets/Audio/Non-FMOD and rebuild registry");
            return false;
        }

        return true;
    }

    private void LogMissingSound(string soundId, string reason)
    {
        if (loggedMissingSounds.Add(soundId))
        {
            Debug.LogWarning($"[AudioManager] Cannot play '{soundId}': {reason}.");
        }
        else if (debugLogging)
        {
            Debug.Log($"[AudioManager] Still skipping '{soundId}' ({reason}).");
        }
    }

    private void LogPlayFailure(string soundId, string reason)
    {
        if (!debugLogging)
        {
            return;
        }

        Debug.LogWarning($"[AudioManager] Failed to play '{soundId}': {reason}. ActiveVoices={CountActiveVoices()}/{poolSize}.");
    }

    private void LogPlaySuccess(string soundId, AudioClip clip, float volume, Vector3 position, string mode)
    {
        if (!debugLogging || !debugLogSuccessfulPlays)
        {
            return;
        }

        Debug.Log($"[AudioManager] Playing '{soundId}' clip='{clip.name}' {mode} vol={volume:F2} pos={position}");
    }

    private void LogDebug(string message)
    {
        if (debugLogging)
        {
            Debug.Log($"[AudioManager] {message}");
        }
    }

    private void LogInitSummary()
    {
        if (!debugLogging)
        {
            return;
        }

        int total = registry.entries.Count;
        int empty = 0;
        var emptyIds = new List<string>();
        foreach (AudioSoundRegistry.SoundEntry entry in registry.entries)
        {
            if (entry.clips == null || entry.clips.Length == 0)
            {
                empty++;
                if (emptyIds.Count < 8)
                {
                    emptyIds.Add(entry.soundId);
                }
            }
        }

        string emptyPreview = emptyIds.Count > 0 ? string.Join(", ", emptyIds) : "none";
        if (empty > emptyIds.Count)
        {
            emptyPreview += $", +{empty - emptyIds.Count} more";
        }

        Debug.Log($"[AudioManager] Registry loaded: {total} entries, {empty} without clips. Empty examples: {emptyPreview}");
    }

    private static bool EnsureCanHearAudio(string soundId)
    {
        if (CountAudioListeners() > 0)
        {
            return true;
        }

        Debug.LogError(
            $"[AudioManager] Cannot play '{soundId}': no AudioListener in the scene. " +
            "Make sure the Main Camera has an AudioListener component.");
        return false;
    }

    private static int CountAudioListeners()
    {
#if UNITY_2023_1_OR_NEWER
        return Object.FindObjectsByType<AudioListener>().Length;
#else
        return Object.FindObjectsOfType<AudioListener>().Length;
#endif
    }

    private int CountActiveVoices()
    {
        int count = 0;
        foreach (PooledVoice voice in pool)
        {
            if (voice.InUse)
            {
                count++;
            }
        }

        return count;
    }

    private bool TryAcquireVoice(
        string soundId,
        AudioSoundRegistry.SoundEntry entry,
        Vector3 position,
        Transform follow,
        bool isLoop,
        float volume,
        out PooledVoice voice)
    {
        voice = null;
        EnforceVoiceLimit(soundId, entry, position);

        voice = FindFreeVoice();
        if (voice == null)
        {
            voice = StealLowestPriorityVoice();
        }

        if (voice == null)
        {
            return false;
        }

        if (voice.InUse)
        {
            ReleaseVoice(voice);
        }

        voice.InUse = true;
        voice.IsLoop = isLoop;
        voice.SoundId = soundId;
        voice.Follow = follow;
        voice.StartTime = Time.time;
        voice.BaseVolume = volume;
        voice.Priority = entry.priority;
        voice.Generation++;

        if (!activeVoicesBySound.TryGetValue(soundId, out List<PooledVoice> list))
        {
            list = new List<PooledVoice>();
            activeVoicesBySound[soundId] = list;
        }

        list.Add(voice);
        return true;
    }

    private void EnforceVoiceLimit(string soundId, AudioSoundRegistry.SoundEntry entry, Vector3 position)
    {
        if (!activeVoicesBySound.TryGetValue(soundId, out List<PooledVoice> list))
        {
            return;
        }

        list.RemoveAll(v => v == null || !v.InUse);

        while (list.Count >= entry.maxConcurrent)
        {
            PooledVoice victim = PickOverflowVictim(list, entry.overflowStrategy, position);
            if (victim == null)
            {
                if (debugLogging && entry.overflowStrategy == AudioSoundRegistry.OverflowStrategy.DropNew)
                {
                    Debug.Log(
                        $"[AudioManager] Voice limit reached for '{soundId}' " +
                        $"({list.Count}/{entry.maxConcurrent}, strategy=DropNew). New play may steal a global pool slot.");
                }

                break;
            }

            if (debugLogging)
            {
                Debug.Log($"[AudioManager] Stealing voice for '{soundId}' (strategy={entry.overflowStrategy}, active={list.Count}/{entry.maxConcurrent}).");
            }

            ReleaseVoice(victim);
            list.Remove(victim);
        }
    }

    private static PooledVoice PickOverflowVictim(
        List<PooledVoice> voices,
        AudioSoundRegistry.OverflowStrategy strategy,
        Vector3 position)
    {
        if (voices.Count == 0)
        {
            return null;
        }

        switch (strategy)
        {
            case AudioSoundRegistry.OverflowStrategy.StealQuietest:
            {
                PooledVoice quietest = voices[0];
                foreach (PooledVoice voice in voices)
                {
                    if (voice.Source.volume < quietest.Source.volume)
                    {
                        quietest = voice;
                    }
                }

                return quietest;
            }
            case AudioSoundRegistry.OverflowStrategy.StealOldest:
            {
                PooledVoice oldest = voices[0];
                foreach (PooledVoice voice in voices)
                {
                    if (voice.StartTime < oldest.StartTime)
                    {
                        oldest = voice;
                    }
                }

                return oldest;
            }
            default:
                return null;
        }
    }

    private PooledVoice FindFreeVoice()
    {
        foreach (PooledVoice voice in pool)
        {
            if (!voice.InUse)
            {
                return voice;
            }
        }

        return null;
    }

    private PooledVoice StealLowestPriorityVoice()
    {
        PooledVoice candidate = null;
        foreach (PooledVoice voice in pool)
        {
            if (!voice.InUse || voice.IsLoop)
            {
                continue;
            }

            if (candidate == null || voice.Priority < candidate.Priority)
            {
                candidate = voice;
            }
        }

        return candidate;
    }

    private PooledVoice GetLastStartedVoice(string soundId)
    {
        if (!activeVoicesBySound.TryGetValue(soundId, out List<PooledVoice> list) || list.Count == 0)
        {
            return null;
        }

        return list[list.Count - 1];
    }

    private bool TryGetVoice(AudioHandle handle, out PooledVoice voice)
    {
        voice = null;
        if (!handle.IsValid || handle.SourceIndex < 0 || handle.SourceIndex >= pool.Count)
        {
            return false;
        }

        voice = pool[handle.SourceIndex];
        return voice.InUse && voice.Generation == handle.Generation;
    }

    private static void ConfigureSource(
        AudioSource source,
        AudioSoundRegistry.SoundEntry entry,
        AudioClip clip,
        float volume,
        bool force2D)
    {
        source.clip = clip;
        source.volume = volume;
        source.pitch = 1f;
        source.loop = entry.loop;
        source.spatialBlend = force2D || entry.is2D ? 0f : 1f;
        source.minDistance = entry.minDistance;
        source.maxDistance = entry.maxDistance;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
    }

    private IEnumerator ReturnWhenFinished(PooledVoice voice)
    {
        yield return null;
        while (voice.InUse && voice.Source.isPlaying)
        {
            yield return null;
        }

        if (voice.InUse && !voice.IsLoop)
        {
            ReleaseVoice(voice);
        }
    }

    private IEnumerator FadeOutAndRelease(PooledVoice voice, float fadeOut)
    {
        float startVolume = voice.Source.volume;
        float elapsed = 0f;
        while (elapsed < fadeOut)
        {
            elapsed += Time.deltaTime;
            voice.Source.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOut);
            yield return null;
        }

        ReleaseVoice(voice);
    }

    private IEnumerator FadeMusicOut(float fadeOut)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;
        while (elapsed < fadeOut)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOut);
            yield return null;
        }

        musicSource.Stop();
        currentMusicId = null;
        musicFadeCoroutine = null;
    }

    private void ReleaseVoice(PooledVoice voice)
    {
        if (voice.ReturnCoroutine != null)
        {
            StopCoroutine(voice.ReturnCoroutine);
            voice.ReturnCoroutine = null;
        }

        string releasedSoundId = voice.SoundId;

        voice.Source.Stop();
        voice.Source.clip = null;
        voice.Source.loop = false;
        voice.InUse = false;
        voice.IsLoop = false;
        voice.Follow = null;
        voice.SoundId = null;

        if (releasedSoundId != null && activeVoicesBySound.TryGetValue(releasedSoundId, out List<PooledVoice> list))
        {
            list.Remove(voice);
        }
    }
}

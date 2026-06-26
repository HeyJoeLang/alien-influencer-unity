#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AudioSoundRegistryBuilder
{
    private const string RegistryPath = "Assets/Resources/AudioSoundRegistry.asset";
    private const string NonFmodRoot = "Assets/Audio/Non-FMOD";

    [MenuItem("Audio/Build Sound Registry From Non-FMOD")]
    public static void BuildRegistry()
    {
        EnsureResourcesFolder();

        AudioSoundRegistry registry = AssetDatabase.LoadAssetAtPath<AudioSoundRegistry>(RegistryPath);
        if (registry == null)
        {
            registry = ScriptableObject.CreateInstance<AudioSoundRegistry>();
            AssetDatabase.CreateAsset(registry, RegistryPath);
        }

        registry.entries = new List<AudioSoundRegistry.SoundEntry>
        {
            Entry(AudioSoundIds.SoundDesign.Defenses.ForceFieldActivation,
                "Defenses/Force Field Activation", maxConcurrent: 3),
            Entry(AudioSoundIds.SoundDesign.Defenses.ForceFieldBounce,
                "Defenses/Force Field Bounce", maxConcurrent: 6, overflow: AudioSoundRegistry.OverflowStrategy.StealOldest),

            Entry(AudioSoundIds.SoundDesign.Destruction.BuildingDebris,
                "Destruction/Building Debris", maxConcurrent: 4),
            Entry(AudioSoundIds.SoundDesign.Destruction.BuildingExplosion,
                "Destruction/Building Explosion", maxConcurrent: 6, overflow: AudioSoundRegistry.OverflowStrategy.StealOldest),
            Entry(AudioSoundIds.SoundDesign.Destruction.Fire,
                "Destruction/Fire 1.wav", loop: true, maxConcurrent: 12),
            Entry(AudioSoundIds.SoundDesign.Destruction.MissileImpactExplosion,
                "Destruction/Missile Impact Explosion", maxConcurrent: 6),
            Entry(AudioSoundIds.SoundDesign.Destruction.DeflectedMissileExplosion,
                "Destruction/Deflected Missile Explosion", maxConcurrent: 4),

            Entry(AudioSoundIds.SoundDesign.PowerUps.CollectMegaLaserCharge,
                "Power Ups/Collect Mega Laser Charge.wav"),
            Entry(AudioSoundIds.SoundDesign.PowerUps.CollectUfoMissileCharge,
                "Power Ups/Collect UFO Missile Charge.wav"),

            Entry(AudioSoundIds.SoundDesign.UI.PlayGame,
                "UI/PlayGame.wav", is2D: true),
            Entry(AudioSoundIds.SoundDesign.UI.ResetScore,
                "UI/Reset Score.wav", is2D: true),
            TodoEntry(AudioSoundIds.SoundDesign.UI.MouseHover),
            TodoEntry(AudioSoundIds.SoundDesign.UI.InsertCoin),

            Entry(AudioSoundIds.SoundDesign.Vehicles.UfoHover,
                "Vehicles/Hum.wav", "Vehicles/UFO Hover_Howl.wav",
                loop: true, priority: 80, maxConcurrent: 1),

            Entry(AudioSoundIds.SoundDesign.Weapons.CityMissileFlight,
                "Weapons/UFO Missile Flight/City Missile Flight_Rumble.wav",
                loop: true, maxConcurrent: 8),
            Entry(AudioSoundIds.SoundDesign.Weapons.CityMissileLaunch,
                "Weapons/City Missile Launch_Blast.wav", maxConcurrent: 6),
            Entry(AudioSoundIds.SoundDesign.Weapons.Laser,
                "Weapons/UFO Lasers/Laser.wav", loop: true, is2D: true, maxConcurrent: 1, priority: 90),
            Entry(AudioSoundIds.SoundDesign.Weapons.MegaLaser,
                "Weapons/UFO Lasers/Mega Laser.wav", loop: true, is2D: true, maxConcurrent: 1, priority: 90),
            Entry(AudioSoundIds.SoundDesign.Weapons.UfoMissilesLaunch,
                "Weapons/UFO Missile Launce 1.wav",
                "Weapons/UFO Missile Launce 2.wav",
                "Weapons/UFO Missile Launce 3.wav", maxConcurrent: 4),
            Entry(AudioSoundIds.SoundDesign.Weapons.UfoDamage,
                "Destruction/UFO Damage", maxConcurrent: 2),

            TodoEntry(AudioSoundIds.Music.MenuMusic, loop: true),
            Entry(AudioSoundIds.Music.GameLoop, "Game Loop (3 Minutes With Tail).wav", loop: true, is2D: true),
            TodoEntry(AudioSoundIds.Music.GameLoop2, loop: true),
            TodoEntry(AudioSoundIds.Dialogue.UfoLaughter),
            TodoEntry(AudioSoundIds.Dialogue.UfoPhrases),
        };

        registry.BuildLookup();
        EditorUtility.SetDirty(registry);
        AssetDatabase.SaveAssets();
        Debug.Log($"Built {registry.entries.Count} sound registry entries at {RegistryPath}");
    }

    private static void EnsureResourcesFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
    }

    private static AudioSoundRegistry.SoundEntry Entry(
        string soundId,
        string pathOrFolder,
        int maxConcurrent = 4,
        bool is2D = false,
        bool loop = false,
        int priority = 64,
        AudioSoundRegistry.OverflowStrategy overflow = AudioSoundRegistry.OverflowStrategy.DropNew,
        params string[] extraPaths)
    {
        var clips = new List<AudioClip>();
        LoadClips(pathOrFolder, clips);
        foreach (string extra in extraPaths)
        {
            LoadClips(extra, clips);
        }

        return new AudioSoundRegistry.SoundEntry
        {
            soundId = soundId,
            clips = clips.ToArray(),
            is2D = is2D,
            loop = loop,
            priority = priority,
            maxConcurrent = maxConcurrent,
            overflowStrategy = overflow
        };
    }

    private static AudioSoundRegistry.SoundEntry TodoEntry(
        string soundId,
        bool loop = false,
        bool is2D = true)
    {
        return new AudioSoundRegistry.SoundEntry
        {
            soundId = soundId,
            clips = new AudioClip[0],
            is2D = is2D,
            loop = loop,
            maxConcurrent = 1
        };
    }

    private static void LoadClips(string pathOrFolder, List<AudioClip> clips)
    {
        string fullPath = Path.Combine(NonFmodRoot, pathOrFolder).Replace('\\', '/');
        if (fullPath.EndsWith(".wav"))
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(fullPath);
            if (clip != null)
            {
                clips.Add(clip);
            }

            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { fullPath });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
            if (clip != null)
            {
                clips.Add(clip);
            }
        }
    }
}
#endif

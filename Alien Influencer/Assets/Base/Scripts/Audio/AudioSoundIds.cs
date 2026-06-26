/// <summary>
/// Sound identifiers mirroring former FMOD event paths (without the event:/ prefix).
/// </summary>
public static class AudioSoundIds
{
    public static class SoundDesign
    {
        public static class Defenses
        {
            public const string ForceFieldActivation = "Sound Design/Defenses/UFO Force Field Activation";
            public const string ForceFieldBounce = "Sound Design/Defenses/UFO Force Field Bounce";
        }

        public static class Destruction
        {
            public const string BuildingDebris = "Sound Design/Destruction/Building Debris";
            public const string BuildingExplosion = "Sound Design/Destruction/Building Explosion";
            public const string Fire = "Sound Design/Destruction/Fire";
            public const string MissileImpactExplosion = "Sound Design/Destruction/Missile Impact Explosion";
            public const string DeflectedMissileExplosion = "Sound Design/Destruction/Deflected Missile Explosion";
        }

        public static class PowerUps
        {
            public const string CollectMegaLaserCharge = "Sound Design/Power-Ups/Collect Mega Laser Charge";
            public const string CollectUfoMissileCharge = "Sound Design/Power-Ups/Collect UFO Missile Charge";
        }

        public static class UI
        {
            public const string PlayGame = "Sound Design/UI/PlayGame";
            public const string ResetScore = "Sound Design/UI/ResetScore";
            // TODO: add Non-FMOD export for Sound Design/UI/MouseHover
            public const string MouseHover = "Sound Design/UI/MouseHover";
            // TODO: add Non-FMOD export for Sound Design/UI/InsertCoin
            public const string InsertCoin = "Sound Design/UI/InsertCoin";
        }

        public static class Vehicles
        {
            public const string UfoHover = "Sound Design/Vehicles/UFO Hover";
        }

        public static class Weapons
        {
            public const string CityMissileFlight = "Sound Design/Weapons/City Missile Flight";
            public const string CityMissileLaunch = "Sound Design/Weapons/City Missile Launch";
            public const string Laser = "Sound Design/Weapons/Laser";
            public const string MegaLaser = "Sound Design/Weapons/Mega Laser";
            public const string UfoMissilesLaunch = "Sound Design/Weapons/UFO Missiles Launch";
            public const string UfoDamage = "Sound Design/Weapons/UFO Damage";
        }
    }

    public static class Music
    {
        // TODO: add Non-FMOD export for Music/Menu Music
        public const string MenuMusic = "Music/Menu Music";
        public const string GameLoop = "Music/Game Loop";
        // TODO: add Non-FMOD export for Music/Game Loop 2
        public const string GameLoop2 = "Music/Game Loop 2";
    }

    public static class Dialogue
    {
        // TODO: add Non-FMOD export for Dialogue/UFO Laughter
        public const string UfoLaughter = "Dialogue/UFO Laughter";
        // TODO: add Non-FMOD export for Dialogue/UFO Phrases
        public const string UfoPhrases = "Dialogue/UFO Phrases";
    }
}

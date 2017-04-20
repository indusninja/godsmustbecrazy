using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace Galaxy
{
    public enum AudioState
    {
        Win,
        Lose,
        CometShoot,
        CometHitHabitablePlanet,
        CometHitUnHabitablePlanet,
        ShipLaunch,
        ShipDeath,
        ShipLanding,
        SolarFlare,
        Ambient,
        None
    }

    class Audio
    {
        public static AudioState State = AudioState.Ambient;

        static SoundEffect _WinAudio;
        public static SoundEffectInstance WinAudioInstance;

        static SoundEffect _LoseAudio;
        public static SoundEffectInstance LoseAudioInstance;

        static SoundEffect _CometShootAudio;
        public static SoundEffectInstance CometShootAudioInstance;

        static SoundEffect _CometHitHabitablePlanetAudio;
        public static SoundEffectInstance CometHitHabitablePlanetAudioInstance;

        static SoundEffect _CometHitUnHabitablePlanetAudio;
        public static SoundEffectInstance CometHitUnHabitablePlanetAudioInstance;

        static SoundEffect _ShipLaunchAudio;
        public static SoundEffectInstance ShipLaunchAudioInstance;

        static SoundEffect _ShipDeathAudio;
        public static SoundEffectInstance ShipDeathAudioInstance;

        static SoundEffect _ShipLandingAudio;
        public static SoundEffectInstance ShipLandingAudioInstance;

        static SoundEffect _SolarFlareAudio;
        public static SoundEffectInstance SolarFlareAudioInstance;

        public static Song AmbientAudio;

        public static void LoadAudio(ContentManager content, String filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    string[] split = line.Split(' ');

                    if (split[0].Equals("[Win]"))
                    {
                        _WinAudio = content.Load<SoundEffect>(split[1]);
                        WinAudioInstance = _WinAudio.CreateInstance();
                        WinAudioInstance.IsLooped = false;
                        WinAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    if (split[0].Equals("[Lose]"))
                    {
                        _LoseAudio = content.Load<SoundEffect>(split[1]);
                        LoseAudioInstance = _LoseAudio.CreateInstance();
                        LoseAudioInstance.IsLooped = false;
                        LoseAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    if (split[0].Equals("[CometShoot]"))
                    {
                        _CometShootAudio = content.Load<SoundEffect>(split[1]);
                        CometShootAudioInstance = _CometShootAudio.CreateInstance();
                        CometShootAudioInstance.IsLooped = false;
                        CometShootAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    if (split[0].Equals("[CometHitHabitablePlanet]"))
                    {
                        _CometHitHabitablePlanetAudio = content.Load<SoundEffect>(split[1]);
                        CometHitHabitablePlanetAudioInstance = _CometHitHabitablePlanetAudio.CreateInstance();
                        CometHitHabitablePlanetAudioInstance.IsLooped = false;
                        CometHitHabitablePlanetAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    if (split[0].Equals("[CometHitUnHabitablePlanet]"))
                    {
                        _CometHitUnHabitablePlanetAudio = content.Load<SoundEffect>(split[1]);
                        CometHitUnHabitablePlanetAudioInstance = _CometHitUnHabitablePlanetAudio.CreateInstance();
                        CometHitUnHabitablePlanetAudioInstance.IsLooped = false;
                        CometHitUnHabitablePlanetAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    if (split[0].Equals("[ShipLaunch]"))
                    {
                        _ShipLaunchAudio = content.Load<SoundEffect>(split[1]);
                        ShipLaunchAudioInstance = _ShipLaunchAudio.CreateInstance();
                        ShipLaunchAudioInstance.IsLooped = false;
                        ShipLaunchAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    if (split[0].Equals("[ShipDeath]"))
                    {
                        _ShipDeathAudio = content.Load<SoundEffect>(split[1]);
                        ShipDeathAudioInstance = _ShipDeathAudio.CreateInstance();
                        ShipDeathAudioInstance.IsLooped = false;
                        ShipDeathAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    if (split[0].Equals("[ShipLanding]"))
                    {
                        _ShipLandingAudio = content.Load<SoundEffect>(split[1]);
                        ShipLandingAudioInstance = _ShipLandingAudio.CreateInstance();
                        ShipLandingAudioInstance.IsLooped = false;
                        ShipLandingAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    if (split[0].Equals("[SolarFlare]"))
                    {
                        _SolarFlareAudio = content.Load<SoundEffect>(split[1]);
                        SolarFlareAudioInstance = _SolarFlareAudio.CreateInstance();
                        SolarFlareAudioInstance.IsLooped = false;
                        SolarFlareAudioInstance.Volume = float.Parse(split[2]) / 100f;
                    }
                    if (split[0].Equals("[Ambient]"))
                    {
                        AmbientAudio = content.Load<Song>(split[1]);
                    }
                }
            }
        }
    }
}

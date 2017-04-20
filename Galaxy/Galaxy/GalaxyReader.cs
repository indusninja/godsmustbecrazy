using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace Galaxy
{
    internal static class GalaxyReader
    {
        private static Dictionary<string, string> _settings;

        public static Dictionary<string, string> Settings{ 
            get{
                if (_settings == null)
                {
                    _settings = new Dictionary<string, string>();
                    LoadSettings(_settings);
                }

                return _settings;
            }
        }
        public static bool LoadLevel(GalaxyGame g, bool clear, string levelName){
            if (clear) g.Components.Clear();
            if (!File.Exists(levelName))
            {
                System.Diagnostics.Trace.Write("Level does not exist: " + levelName);
                return false;
            }
            string[] level = File.ReadAllLines(levelName);
            Vector2 lastStarPos = new Vector2(GalaxyGame.ScreenWidth / 2, GalaxyGame.ScreenWidth / 2);
            foreach (var lvlLine in level)
            {
                IGameComponent gc = ParseLine(lvlLine, g, ref lastStarPos);
                if (gc!=null) g.Components.Add(gc);
            }
            return true;
        }

        private static IGameComponent ParseLine(string lvlLine, GalaxyGame g, ref Vector2 lastStarPos)
        {
            if (lvlLine.IndexOf(';') == -1) return null;
            IGameComponent gc = null;
            string[] lineElements = lvlLine.Split(';');
            switch (lvlLine[0])
            {
                case 'S':
                    if (lineElements.Count() != 5) return gc;
                    lastStarPos = new Vector2(float.Parse(lineElements[3]), float.Parse(lineElements[4]));
                    gc = new Star(g, lineElements[1], float.Parse(lineElements[2]), 
                        ref lastStarPos);
                    break;
                case 'P':
                    if (lineElements.Count() != 7) return gc;
                    HabitationState hs = (lineElements[6] == "C") ? HabitationState.Colonizable : HabitationState.Uninhabitable; 
                    gc = new Planet(g, lineElements[1], float.Parse(lineElements[2]), float.Parse(lineElements[3]),
                        float.Parse(lineElements[4]), lastStarPos, long.Parse(lineElements[5]) * 1000000, hs);
                    break;
                case 'M':
                    if (lineElements.Count() != 5) return gc;
                    gc = new MeteorField(g, lineElements[1], new Vector2(float.Parse(lineElements[2]), float.Parse(lineElements[3])),
                        float.Parse(lineElements[4]));
                    break;
                default:
                    // Comment line or something.
                    break;
            }

            return gc; 
        }

        private static void LoadSettings(Dictionary<string, string> settings)
        {
            string[] lines = File.ReadAllLines(@"Content\settings.txt");
            foreach (var settingLine in lines)
            {
                if (settingLine.Contains('='))
                {
                    string[] setting = settingLine.Split(new char[]{'='}, 2);
                    settings[setting[0]] = setting[1];
                }
            }
        }
    }
}

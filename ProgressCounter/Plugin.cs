using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IllusionPlugin;

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Globalization;

namespace ProgressCounter
{
    public class Plugin : IPlugin
    {
        public string Name => "ProgressCounter";
        public string Version => "3.1";

        private readonly string[] env = { "DefaultEnvironment", "BigMirrorEnvironment", "TriangleEnvironment", "NiceEnvironment" };
        bool _init = false;

        public static Vector3 scoreCounterPosition = new Vector3(3.25f, 0.5f, 7f);
        public static bool progressTimeLeft = false;
        public static Vector3 progressCounterPosition = new Vector3(0.25f, -2f, 7.5f);

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            if (ModPrefs.GetString("BeatSaberProgressCounter", "scorePosition") == "")
            {
                ModPrefs.SetString("BeatSaberProgressCounter", "scorePosition", 
                    FormattableString.Invariant(
                        $"{scoreCounterPosition.x:0.00},{scoreCounterPosition.y:0.00},{scoreCounterPosition.z:0.00}"));
            }
            else
            {
                var scoreString = ModPrefs.GetString("BeatSaberProgressCounter", "scorePosition");
                var scorePosVals = scoreString.Split(',').Select(f => float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
                scoreCounterPosition = new Vector3(scorePosVals[0], scorePosVals[1], scorePosVals[2]);
            }


            if (ModPrefs.GetString("BeatSaberProgressCounter", "progressPosition") == "")
            {
                ModPrefs.SetString("BeatSaberProgressCounter", "progressPosition", 
                    FormattableString.Invariant(
                        $"{progressCounterPosition.x:0.00},{progressCounterPosition.y:0.00},{progressCounterPosition.z:0.00}"));
            }
            else
            {
                var progressString = ModPrefs.GetString("BeatSaberProgressCounter", "progressPosition");
                var progressPosVals = progressString.Split(',').Select(f => float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
                progressCounterPosition = new Vector3(progressPosVals[0], progressPosVals[1], progressPosVals[2]);
            }


            if (ModPrefs.GetString("BeatSaberProgressCounter", "progressTimeLeft") == "")
            {
                ModPrefs.SetBool("BeatSaberProgressCounter", "progressTimeLeft", progressTimeLeft);
            }
            else
            {
                progressTimeLeft = ModPrefs.GetBool("BeatSaberProgressCounter", "progressTimeLeft");
            }

            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene _, Scene scene)
        {
            if (!env.Contains(scene.name)) return;

            new GameObject("Counter").AddComponent<Counter>();
            new GameObject("ScoreCounter").AddComponent<ScoreCounter>();
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnUpdate()
        {
        }
    }
}

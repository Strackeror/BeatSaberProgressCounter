using IllusionPlugin;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProgressCounter
{
    public class Plugin : IPlugin
    {
        public string Name => "ProgressCounter";
        public string Version => "4.0";

        private readonly string[] env = { "DefaultEnvironment", "BigMirrorEnvironment", "TriangleEnvironment", "NiceEnvironment" };
        private bool _init = false;

        public static bool progressTimeLeft = false;
        public static Vector3 scoreCounterPosition = new Vector3(3.25f, 0.5f, 7f);
        public static Vector3 progressCounterPosition = new Vector3(0.25f, -2f, 7.5f);

        public static int progressCounterDecimalPrecision;

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private string FormatVector(Vector3 v)
        {
            return FormattableString.Invariant($"{v.x:0.00},{v.y:0.00},{v.z:0.00}");
        }

        private Vector3 ReadVector(string s)
        {
            var arr = s.Split(',').Select(f => float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
            return new Vector3(arr[0], arr[1], arr[2]);
        }

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            scoreCounterPosition = ReadVector(ModPrefs.GetString("BeatSaberProgressCounter", "scorePosition", 
                FormatVector(scoreCounterPosition), true));
            progressCounterPosition = ReadVector(ModPrefs.GetString("BeatSaberProgressCounter", "progressPosition", 
                FormatVector(progressCounterPosition), true));

            progressTimeLeft = ModPrefs.GetBool("BeatSaberProgressCounter", "progressTimeLeft", false, true);
            progressCounterDecimalPrecision = ModPrefs.GetInt("BeatSaberProgressCounter", "progressCounterDecimalPrecision", 1, true);

            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene _, Scene scene)
        {
            if (scene.name == "Menu")
            {
                ProgressUI.CreateSettingsUI();
            }

            if (env.Contains(scene.name))
            {
                new GameObject("Counter").AddComponent<Counter>();
                new GameObject("ScoreCounter").AddComponent<ScoreCounter>();
            }
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
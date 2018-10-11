using IllusionPlugin;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
        public static bool scoreCounterEnabled = true;

        public static bool pbTrackerEnabled = true;
        public static int noteCount;
        public static int localHighScore;
        public static float oldNotes = 0;
        public static float oldHighScore = 0;
        public static float pbPercent;

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
            scoreCounterEnabled = ModPrefs.GetBool("BeatSaberProgressCounter", "scoreCounterEnabled", true, true);

            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene _, Scene scene)
        {
            if (scene.name == "Menu")
            {
                ProgressUI.CreateSettingsUI();

                var levelDetails = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
                if (levelDetails != null) levelDetails.didPressPlayButtonEvent += LevelDetails_didPressPlayButtonEvent; ;

            }

            if (env.Contains(scene.name))
            {
                new GameObject("Counter").AddComponent<Counter>();

                if(scoreCounterEnabled == true) new GameObject("ScoreCounter").AddComponent<ScoreCounter>();
            }
        }

        private void LevelDetails_didPressPlayButtonEvent(StandardLevelDetailViewController obj)
        {
            calcLocalPercent();
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

        public static void calcLocalPercent()
        {
            var levelDetails = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();

            int i = 0;
            string noteString = (ReflectionUtil.GetPrivateField<TextMeshProUGUI>(levelDetails, "_notesCountText").text);
            if (!Int32.TryParse(noteString, out i))
                i = 0;
            else
                noteCount = i;
            //Get Player Score
            int j = 0;
            string scoreString = ReflectionUtil.GetPrivateField<TextMeshProUGUI>(levelDetails, "_highScoreText").text;
            if (!Int32.TryParse(scoreString, out j))
                j = 0;
            else
                localHighScore = j;
            //Get Max Score for song
            int songMaxScore = ScoreController.MaxScoreForNumberOfNotes(noteCount);

            //Set / check values of oldNotes, OldScore
            if (oldNotes == 0)
            {
                oldNotes = noteCount;
                oldHighScore = localHighScore;
            }
            else if (oldNotes != noteCount && oldHighScore == localHighScore)
            {
                localHighScore = 0;
            }
            else if (oldNotes != noteCount && localHighScore != 0)
            {
                oldNotes = noteCount;
                oldHighScore = localHighScore;
            }

            float roundMultiple = 100 * (float)(Math.Pow(10, Plugin.progressCounterDecimalPrecision));

            pbPercent = (float)Math.Floor(((localHighScore / (float)songMaxScore) * roundMultiple)) / roundMultiple;
            
        }
    }
}
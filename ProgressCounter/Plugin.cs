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
using UnityEngine.UI;

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
        public static bool scoreCounterEnabled = true;
        public static bool localScoreCounterEnabled = false;

        public static int progressCounterDecimalPrecision;
        public static int progressCounterPositionPreset;
        public static int progressCounterScorePositionPreset;

        public static int localHighScore;
        public static int noteCount;
        public static int songMaxScore;
        public static float localPercent;
        public static float oldNotes = 0;
        public static float oldHighscore = 0;


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

            if (ModPrefs.GetString("BeatSaberProgressCounter", "scoreCounterEnabled") == "")
            {
                ModPrefs.SetBool("BeatSaberProgressCounter", "scoreCounterEnabled", scoreCounterEnabled);
            }
            else
            {
                scoreCounterEnabled = ModPrefs.GetBool("BeatSaberProgressCounter", "scoreCounterEnabled");
            }

            if (ModPrefs.GetString("BeatSaberProgressCounter", "localScoreCounterEnabled") == "")
            {
                ModPrefs.SetBool("BeatSaberProgressCounter", "localScoreCounterEnabled", localScoreCounterEnabled);
            }
            else
            {
                localScoreCounterEnabled = ModPrefs.GetBool("BeatSaberProgressCounter", "localScoreCounterEnabled");
            }



            //Get value for Decimal Precision from modprefs
            progressCounterDecimalPrecision = ModPrefs.GetInt("BeatSaberProgressCounter", "progressCounterDecimalPrecision", 1, true);
            limitRange(ref progressCounterDecimalPrecision, 1, 4);

            //Get Value, Force progressCounterPositionPreset to be within range
            progressCounterPositionPreset = ModPrefs.GetInt("BeatSaberProgressCounter", "progressCounterPositionPreset", 0, true);

            //Get Value, Force progressCounterScorePositionPreset within range
            progressCounterScorePositionPreset = ModPrefs.GetInt("BeatSaberProgressCounter", "progressCounterScorePositionPreset", 0, true);

            //Set position of Score Counter if Preset selected
            setScoreCounterPosition(progressCounterScorePositionPreset);

            //Set modprefs for timer position if Preset Selected
            setProgressCounterPosition(progressCounterPositionPreset);

            
            SceneManager.activeSceneChanged += OnSceneChanged;

        }

        private void OnSceneChanged(Scene _, Scene scene)
        {
            if (scene.name == "Menu")
            {
                ProgressUI.CreateSettingsUI();
                 var levelDetails = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();
                if (levelDetails != null)
                {
                    //    levelDifficulty.didSelectDifficultyEvent += LevelDifficulty_didSelectDifficultyEvent;
                    levelDetails.didPressPlayButtonEvent += LevelDetails_didPressPlayButtonEvent;
                }
                else
                {
                    Log("Object is null");
                }

            }

          
            if (!env.Contains(scene.name)) return;

            new GameObject("Counter").AddComponent<Counter>();

            if (scoreCounterEnabled == true)
            {
                new GameObject("ScoreCounter").AddComponent<ScoreCounter>();
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

        public static void setScoreCounterPosition(int value)
        {
            switch (value)
            {
                case 0: //User
                    var scoreString = ModPrefs.GetString("BeatSaberProgressCounter", "scorePosition");
                    var scorePosVals = scoreString.Split(',').Select(f => float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
                    scoreCounterPosition = new Vector3(scorePosVals[0], scorePosVals[1], scorePosVals[2]);
                    break;

                case 1: //Default
                    scoreCounterPosition = new Vector3(3.25f, 0.5f, 7.0f);
                    break;

                case 2: //Left
                    scoreCounterPosition = new Vector3(-3.25f, -0.3f, 7.0f);
                    break;

                default:

                    progressCounterScorePositionPreset = 0;

                    scoreString = ModPrefs.GetString("BeatSaberProgressCounter", "scorePosition");
                    scorePosVals = scoreString.Split(',').Select(f => float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
                    scoreCounterPosition = new Vector3(scorePosVals[0], scorePosVals[1], scorePosVals[2]);
                    break;
            }

            ModPrefs.SetString("BeatSaberProgressCounter", "scorePosition",
                     FormattableString.Invariant(
                    $"{scoreCounterPosition.x:0.00},{scoreCounterPosition.y:0.00},{scoreCounterPosition.z:0.00}"));
        }

        public static void setProgressCounterPosition(int value)
        {
            switch (value)
            {
                case 0: //User
                    var progressString = ModPrefs.GetString("BeatSaberProgressCounter", "progressPosition");
                    var progressPosVals = progressString.Split(',').Select(f => float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
                    progressCounterPosition = new Vector3(progressPosVals[0], progressPosVals[1], progressPosVals[2]);
                    break;

                case 1: //Default
                    progressCounterPosition = new Vector3(.25f, -2.0f, 7.5f);
                    break;

                case 2: //Top
                    progressCounterPosition = new Vector3(.25f, 3.4f, 7.5f);
                    break;

                case 3: //TopLeft
                    progressCounterPosition = new Vector3(-3.0f, 3.4f, 7f);
                    break;

                case 4: //TopRight
                    progressCounterPosition = new Vector3(3.5f, 3.4f, 7f);
                    break;

                case 5: //BottomLeft
                    progressCounterPosition = new Vector3(-3.0f, -1.75f, 7f);
                    break;

                case 6: //BottomRight
                    progressCounterPosition = new Vector3(3.5f, -1.6f, 7f);
                    break;

                default:

                    progressCounterPositionPreset = 0;

                    progressString = ModPrefs.GetString("BeatSaberProgressCounter", "progressPosition");
                    progressPosVals = progressString.Split(',').Select(f => float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
                    progressCounterPosition = new Vector3(progressPosVals[0], progressPosVals[1], progressPosVals[2]);
                    break;
            }

            ModPrefs.SetString("BeatSaberProgressCounter", "progressPosition",
             FormattableString.Invariant(
           $"{progressCounterPosition.x:0.00},{progressCounterPosition.y:0.00},{progressCounterPosition.z:0.00}"));

        }

        public static void calcLocalPercent()
        {
            var levelDetails = Resources.FindObjectsOfTypeAll<StandardLevelDetailViewController>().FirstOrDefault();

            int i = 0;
            string noteString =( ReflectionUtil.GetPrivateField<TextMeshProUGUI>(levelDetails, "_notesCountText").text);
            if (!Int32.TryParse(noteString, out i))
            i = 0;
            else
            noteCount = i;
            Log(noteCount.ToString());
            //Get Player Score
            int j = 0;
            string scoreString = ReflectionUtil.GetPrivateField<TextMeshProUGUI>(levelDetails, "_highScoreText").text;
            if (!Int32.TryParse(scoreString, out j))
                j = 0;
            else
                localHighScore = j;
            Log(localHighScore.ToString());
            //Get Max Score for song
           songMaxScore = ScoreController.MaxScoreForNumberOfNotes(noteCount);

            //Set / check values of oldNotes, OldScore
            if (oldNotes == 0)
            {
                oldNotes = noteCount;
                oldHighscore = localHighScore;
            }
            if(oldNotes != noteCount && oldHighscore == localHighScore)
            {
                localHighScore = 0;
            }
            

            Log(oldNotes.ToString() + "  "  + oldHighscore.ToString());



            Log(songMaxScore.ToString());
            float roundMultiple = 100 * (float)(Math.Pow(10, Plugin.progressCounterDecimalPrecision));

            localPercent = (float)Math.Floor(((localHighScore / (float)songMaxScore) * roundMultiple)) / roundMultiple;
            Log(localPercent.ToString());
            TextMeshProUGUI filler = new TextMeshProUGUI();
            filler.text = "";

        }

        public void limitRange(ref int value, int min, int max)
        {
            if (value > max)
            {
                value = max;
            }

            else if (value < min)
            {
                value = min;
            }

        }

        public static void Log(string message)
        {
            Console.WriteLine("[{0}] {1}", "ProgressCounter", message);
        }

    }
}

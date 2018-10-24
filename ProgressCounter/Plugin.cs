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
        private static HMAsyncRequest _asyncRequest;

        public static bool progressTimeLeft = false;
        public static Vector3 scoreCounterPosition = new Vector3(3.25f, 0.5f, 7f);
        public static Vector3 progressCounterPosition = new Vector3(0.25f, -2f, 7.5f);

        public static int progressCounterDecimalPrecision;
        public static bool scoreCounterEnabled = true;

        public static bool pbTrackerEnabled = true;
        public static int noteCount;
        public static int localHighScore;
        public static float pbPercent;

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
     
        private void OnSceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (env.Contains(scene.name))
            {
                GetSongInfo();

                new GameObject("Counter").AddComponent<Counter>();

                if (scoreCounterEnabled) new GameObject("ScoreCounter").AddComponent<ScoreCounter>();
            }
        }

        private void OnSceneChanged(Scene _, Scene scene)
        {
            if (scene.name == "Menu") ProgressUI.CreateSettingsUI();
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

        public static void GetSongInfo()
        {
            var mainGameSceneSetupData = Resources.FindObjectsOfTypeAll<MainGameSceneSetupData>().First();
            var playerLevelStatsData = PersistentSingleton<GameDataModel>.instance.gameDynamicData.GetCurrentPlayerDynamicData().GetPlayerLevelStatsData(mainGameSceneSetupData.difficultyLevel.level.levelID, mainGameSceneSetupData.difficultyLevel.difficulty, mainGameSceneSetupData.gameplayMode);

            //Get notes count
            noteCount = mainGameSceneSetupData.difficultyLevel.beatmapData.notesCount;
            
            //Get Player Score
            localHighScore = playerLevelStatsData.validScore ? playerLevelStatsData.highScore : 0;

            //If we couldn't grab a local score, we'll try to grab one from the leaderboards
            if (localHighScore == 0)
            {
                string leaderboardID = LeaderboardsModel.GetLeaderboardID(mainGameSceneSetupData.difficultyLevel, mainGameSceneSetupData.gameplayMode);
                if (_asyncRequest != null)
                {
                    _asyncRequest.Cancel();
                }
                _asyncRequest = new HMAsyncRequest();

                //Note: I'm leaving "around" as 10 intentionally so that this "looks" like a normal score request
                //Don't judge. Old habits die hard.
                PersistentSingleton<PlatformLeaderboardsModel>.instance.GetScoresAroundPlayer(leaderboardID, 10, _asyncRequest, LeaderboardsResultsReturned);
            }
            CalculatePercentage();
        }

        private static void CalculatePercentage()
        {
            //Get Max Score for song
            int songMaxScore = ScoreController.MaxScoreForNumberOfNotes(noteCount);

            float roundMultiple = 100 * (float)Math.Pow(10, progressCounterDecimalPrecision);

            pbPercent = (float)Math.Floor(localHighScore / (float)songMaxScore * roundMultiple) / roundMultiple;

            //If the ScoreCounter has already been created, we'll have to set the Personal Best from out here
            var scoreCounter = Resources.FindObjectsOfTypeAll<ScoreCounter>().FirstOrDefault();
            if (scoreCounter != null) scoreCounter.SetPersonalBest(pbPercent);
        }

        //Callback for a leaderboard score request. Sets the PB score to the returned one
        public static void LeaderboardsResultsReturned(PlatformLeaderboardsModel.GetScoresResult result, PlatformLeaderboardsModel.LeaderboardScore[] scores, int playerScoreIndex)
        {
            if (result == PlatformLeaderboardsModel.GetScoresResult.OK && playerScoreIndex > 0)
            {
                localHighScore = scores.ElementAt(playerScoreIndex).score;
                CalculatePercentage();
            }
        }
    }
}
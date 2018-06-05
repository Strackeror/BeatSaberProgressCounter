using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IllusionPlugin;

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace ProgressCounter
{
    public class Counter : MonoBehaviour
    {

        TextMeshPro _timeMesh;
        AudioTimeSyncController _audioTimeSync;

        void Awake()
        {
            _timeMesh = this.gameObject.AddComponent<TextMeshPro>();
            _timeMesh.text = "0%";
            _timeMesh.fontSize = 4;
            _timeMesh.color = Color.white;
            _timeMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _timeMesh.rectTransform.position = new Vector3(-3.6f, -3f, 7f) - new Vector3(_timeMesh.rectTransform.offsetMin.x, _timeMesh.rectTransform.offsetMin.y);
            _audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
        }

        void Update()
        {
            _timeMesh.text = $"{(_audioTimeSync.songTime / _audioTimeSync.songLength) * 100:N1}%";
        }
    }

    public class ScoreCounter : MonoBehaviour
    {
        TextMeshPro _scoreMesh;
        ScoreController _scoreController;

        int _maxPossibleScore = 1;

        void Start()
        {
            _scoreMesh = this.gameObject.AddComponent<TextMeshPro>();
            _scoreMesh.text = "0.0%";
            _scoreMesh.fontSize = 4;
            _scoreMesh.color = Color.white;
            _scoreMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _scoreMesh.rectTransform.position = new Vector3(2.9f, -4.5f, 7f) - new Vector3(_scoreMesh.rectTransform.offsetMin.x, _scoreMesh.rectTransform.offsetMin.y);
            _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();

            GameplayManager gameplayManager = FindObjectOfType<GameplayManager>();
            if (gameplayManager != null)
            {
                GameSongController songController = ReflectionUtil.GetPrivateField<GameSongController>(gameplayManager, "_gameSongController");
                if (songController != null)
                {
                    SongData songData = ReflectionUtil.GetPrivateField<SongData>(songController, "_songData");
                    if (songData != null)
                    {
                        this._maxPossibleScore = ScoreController.MaxScoreForNumberOfNotes(songData.notesCount);
                    }
                }
            }

            if (_scoreController != null)
            {
                _scoreController.scoreDidChangeEvent += UpdateScore;
            }

        }

        void UpdateScore(int score)
        {
            if (_scoreMesh != null)
                _scoreMesh.text = (((float)score / (float)_maxPossibleScore) * 100.0f).ToString("F1") + "%";
        }
    }

    public class Plugin : IPlugin
    {
        public string Name => "Progress";

        public string Version => "1.0";

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        bool _init = false;

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene _, Scene scene)
        {

            if (scene.buildIndex != 4) return;
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

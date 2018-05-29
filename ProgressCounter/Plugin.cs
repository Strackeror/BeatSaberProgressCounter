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

        TextMeshPro _mesh;
        AudioTimeSyncController _audioTimeSync;

        void Awake()
        {
            _mesh = this.gameObject.AddComponent<TextMeshPro>();
            _mesh.text = "100%";
            _mesh.fontSize = 4;
            _mesh.color = Color.white;
            _mesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _mesh.rectTransform.position = new Vector3(-3.6f, -3f, 7f) - new Vector3(_mesh.rectTransform.offsetMin.x, _mesh.rectTransform.offsetMin.y);
            Console.WriteLine($"{_mesh.rectTransform.offsetMin}");
            var combocontroller = Resources.FindObjectsOfTypeAll<ComboUIController>().FirstOrDefault() as ComboUIController;
            _audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
            
        }

        void Update()
        {
            _mesh.text = $"{(_audioTimeSync.songTime / _audioTimeSync.songLength) * 100:N1}%";
        }
    }

    public class Plugin : IPlugin
    {
        public string Name => "Progress";

        public string Version => "1.0";

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnScenChanged;
        }

        bool _init = false;

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;
            SceneManager.activeSceneChanged += OnScenChanged;
        }

        private void OnScenChanged(Scene _, Scene scene)
        {
            if (scene.buildIndex != 4) return;
            new GameObject("Counter").AddComponent<Counter>();
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

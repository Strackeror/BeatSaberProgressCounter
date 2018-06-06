using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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
}

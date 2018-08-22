﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;

namespace ProgressCounter
{
    public class Counter : MonoBehaviour
    {

        TextMeshPro _timeMesh;
        AudioTimeSyncController _audioTimeSync;
        Image _image;

        IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                _audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();

                if (_audioTimeSync == null)
                    yield return new WaitForSeconds(0.01f);
                else
                    loaded = true;
            }

            Init();
        }

        private void Awake()
        {
            StartCoroutine(WaitForLoad());
        }

        void Init()
        {
            _timeMesh = this.gameObject.AddComponent<TextMeshPro>();
            _timeMesh.text = "0:00";
            _timeMesh.fontSize = 4;
            _timeMesh.color = Color.white;
            _timeMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _timeMesh.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
            _timeMesh.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
            _timeMesh.rectTransform.position = new Vector3(0.25f, -2f, 7.5f);

            var image = ReflectionUtil.GetPrivateField<Image>(
                Resources.FindObjectsOfTypeAll<ScoreMultiplierUIController>().First(), "_multiplierProgressImage");

            GameObject g = new GameObject();
            Canvas canvas = g.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler cs = g.AddComponent<CanvasScaler>();
            cs.scaleFactor = 10.0f;
            cs.dynamicPixelsPerUnit = 10f;
            GraphicRaycaster gr = g.AddComponent<GraphicRaycaster>();
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);

            GameObject g2 = new GameObject();
            _image = g2.AddComponent<Image>();
            g2.transform.parent = g.transform;
            g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.5f);
            g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.5f);
            g2.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);

            _image.sprite = image.sprite;
            _image.type = Image.Type.Filled;
            _image.fillMethod = Image.FillMethod.Radial360;
            _image.fillOrigin = (int)Image.Origin360.Top;
            _image.fillClockwise = true;


            GameObject g3 = new GameObject();
            var bg = g3.AddComponent<Image>();
            g3.transform.parent = g.transform;
            g3.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.5f);
            g3.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.5f);
            g3.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);

            bg.sprite = image.sprite;
            bg.CrossFadeAlpha(0.05f, 1f, false);

            g.GetComponent<RectTransform>().SetParent(this.transform, false);
            g.transform.localPosition = new Vector3(-0.25f, .25f, 0f);
        }

        void Update()
        {
            if (_audioTimeSync == false)
            {
                _audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
                return;
            }

            _timeMesh.text = $"{Math.Floor(_audioTimeSync.songTime / 60):N0}:{Math.Floor(_audioTimeSync.songTime % 60):00}";
            _image.fillAmount = _audioTimeSync.songTime / _audioTimeSync.songLength;
        }
    }
}

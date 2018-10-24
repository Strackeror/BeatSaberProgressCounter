
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace ProgressCounter
{
    public class ScoreCounter : MonoBehaviour
    {
        TextMeshPro _scoreMesh;
        ScoreController _scoreController;
        BeatmapObjectExecutionRatingsRecorder _objectRatingRecorder;

        GameObject _RankObject;
        TextMeshPro _RankText;

        GameObject _PbTrackerObject;
        TextMeshPro _PbTrackerText;
        int _maxPossibleScore = 0;
        float roundMultiple;

        IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                _objectRatingRecorder = FindObjectOfType<BeatmapObjectExecutionRatingsRecorder>();

                if (_scoreController == null || _objectRatingRecorder == null)
                    yield return new WaitForSeconds(0.1f);
                else
                    loaded = true;
            }

            Init();
        }

        void Awake()
        {
            StartCoroutine(WaitForLoad());
        }

        private void Init()
        {
            roundMultiple = (float)Math.Pow(100, Plugin.progressCounterDecimalPrecision);

            _scoreMesh = this.gameObject.AddComponent<TextMeshPro>();
            _scoreMesh.text = "100.0%";
            _scoreMesh.fontSize = 3;
            _scoreMesh.color = Color.white;
            _scoreMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _scoreMesh.alignment = TextAlignmentOptions.Center;
            _scoreMesh.rectTransform.position = Plugin.scoreCounterPosition;

            _RankObject = new GameObject();
            _RankText = _RankObject.AddComponent<TextMeshPro>();
            _RankText.text = "SSS";
            _RankText.fontSize = 4;
            _RankText.color = Color.white;
            _RankText.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _RankText.alignment = TextAlignmentOptions.Center;
            _RankText.rectTransform.position = _scoreMesh.rectTransform.position + new Vector3(0f, -0.4f, 0f);

            if (Plugin.pbTrackerEnabled)
            {
                SetPersonalBest(Plugin.pbPercent);
            }
            if (_scoreController != null)
                _scoreController.scoreDidChangeEvent += UpdateScore;
        }

        //Sometimes a leaderboard request will run past creation of this object.
        //In that case, we'll need to be able to change the personal best from the outside
        public void SetPersonalBest(float pb)
        {
            //Force personal best percent to round down to decimal precision
            pb = (float)Math.Floor(pb * roundMultiple) / roundMultiple;

            if (_PbTrackerObject == null)
            {
                _PbTrackerObject = new GameObject("PB Tracker");
                _PbTrackerText = _PbTrackerObject.AddComponent<TextMeshPro>();
                _PbTrackerText.fontSize = 2;
                _PbTrackerText.color = Color.white;
                _PbTrackerText.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
                _PbTrackerText.alignment = TextAlignmentOptions.Center;
            }
            if (pb == 0) _PbTrackerText.text = "--";
            else _PbTrackerText.text = "PB: " + (Mathf.Clamp(pb, 0.0f, 1.0f) * 100.0f).ToString("F" + Plugin.progressCounterDecimalPrecision) + "%";
            _PbTrackerText.rectTransform.position = _scoreMesh.rectTransform.position + new Vector3(0f, -0.8f, 0f);
        }

        public string GetRank(int score, float prec)
        {
            if (score >= _maxPossibleScore)
            {
                return "SSS";
            }
            if (prec > 0.9f)
            {
                return "SS";
            }
            if (prec > 0.8f)
            {
                return "S";
            }
            if (prec > 0.65f)
            {
                return "A";
            }
            if (prec > 0.5f)
            {
                return "B";
            }
            if (prec > 0.35f)
            {
                return "C";
            }
            if (prec > 0.2f)
            {
                return "D";
            }
            return "E";
        }

        public void UpdateScore(int score)
        {

            if (_objectRatingRecorder != null)
            {
                List<BeatmapObjectExecutionRating> _ratings = ReflectionUtil.GetPrivateField<List<BeatmapObjectExecutionRating>>(_objectRatingRecorder, "_beatmapObjectExecutionRatings");
                if (_ratings != null)
                {
                    int notes = 0;
                    foreach (BeatmapObjectExecutionRating rating in _ratings)
                    {
                        if (rating.beatmapObjectRatingType == BeatmapObjectExecutionRating.BeatmapObjectExecutionRatingType.Note)
                            notes++;
                    }
                    _maxPossibleScore = ScoreController.MaxScoreForNumberOfNotes(notes);
                }
            }

            if (_scoreMesh != null)
            {
                if (_maxPossibleScore == 0)
                {
                    _scoreMesh.text = "100.0%";
                    _RankText.text = "SSS";
                }
                else
                {
                    float ratio = score / (float)_maxPossibleScore;
                    //Force percent to round down to decimal precision
                    ratio = (float)Math.Floor(ratio * roundMultiple) / roundMultiple;
                    if (Plugin.pbPercent != 0 && Plugin.pbPercent > ratio)
                        _scoreMesh.color = Color.red;
                    else if (Plugin.pbPercent != 0 && Plugin.pbPercent < ratio)
                        _scoreMesh.color = Color.white;

                    _scoreMesh.text = (Mathf.Clamp(ratio, 0.0f, 1.0f) * 100.0f).ToString("F" + Plugin.progressCounterDecimalPrecision) + "%";
                    _RankText.text = GetRank(score, ratio);
                }
            }
        }
    }
}
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
        float precision = Plugin.progressCounterDecimalPrecision;
        float roundMultiple;

        GameObject _RankObject;
        TextMeshPro _RankText;

        GameObject _localRankObject;
        TextMeshPro _localRankText;

        int _maxPossibleScore = 0;

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

            if (Plugin.localScoreCounterEnabled == true)
            {
                _localRankObject = new GameObject();
                _localRankText = _localRankObject.AddComponent<TextMeshPro>();
                _localRankText.text = "PB: " + (Mathf.Clamp(Plugin.localPercent, 0.0f, 1.0f) * 100.0f).ToString("F" + precision) + "%";
                if (Plugin.localPercent == 0) _localRankText.text = "--";
                _localRankText.fontSize = 4;
                _localRankText.color = Color.white;
                _localRankText.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
                _localRankText.alignment = TextAlignmentOptions.Center;
                _localRankText.rectTransform.position = _scoreMesh.rectTransform.position + new Vector3(0f, -0.8f, 0f);
            }
            if (_scoreController != null)
                _scoreController.scoreDidChangeEvent += UpdateScore;
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
            roundMultiple = 100 * (float)(Math.Pow(10, this.precision));
            float percent = (float)Math.Floor( ( ((float)score / (float)_maxPossibleScore) ) * roundMultiple) /  roundMultiple;
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
                    
                    _scoreMesh.text = (Mathf.Clamp(percent, 0.0f, 1.0f) * 100.0f).ToString("F" + precision) + "%";
                    _RankText.text = GetRank(score, percent);

                }

            }
        }
    }
}

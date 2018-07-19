using System;
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

        int _maxPossibleScore = 0;

        private void Awake()
        {
            _scoreMesh = this.gameObject.AddComponent<TextMeshPro>();
            _scoreMesh.text = "100.0%";
            _scoreMesh.fontSize = 3;
            _scoreMesh.color = Color.white;
            _scoreMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _scoreMesh.alignment = TextAlignmentOptions.Center;
            _scoreMesh.rectTransform.position = new Vector3(3.25f, 0.5f, 7f);

            _RankObject = new GameObject();
            _RankText = _RankObject.AddComponent<TextMeshPro>();
            _RankText.text = "SSS";
            _RankText.fontSize = 4;
            _RankText.color = Color.white;
            _RankText.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _RankText.alignment = TextAlignmentOptions.Center;
            _RankText.rectTransform.position = new Vector3(3.25f, 0.1f, 7f);

            _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
            _objectRatingRecorder = FindObjectOfType<BeatmapObjectExecutionRatingsRecorder>();

            if (_scoreController != null)
            {
                _scoreController.scoreDidChangeEvent += UpdateScore;
            }

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
            float percent = (float)score / (float)_maxPossibleScore;

            if (_objectRatingRecorder != null)
            {
                List<BeatmapObjectExecutionRating> _ratings = ReflectionUtil.GetPrivateField<List<BeatmapObjectExecutionRating>>(_objectRatingRecorder, "_songObjectExecutionRatings");
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
                    _scoreMesh.text = (Mathf.Clamp(percent, 0.0f, 1.0f) * 100.0f).ToString("F1") + "%";
                    _RankText.text = GetRank(score, percent);
                }

            }
        }
    }
}

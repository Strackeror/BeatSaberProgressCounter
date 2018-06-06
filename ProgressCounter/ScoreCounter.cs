using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace ProgressCounter
{
    /*public class ScoreCounter : MonoBehaviour
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

    }*/
    
    public class ScoreCounter : MonoBehaviour
    {
        TextMeshPro _scoreMesh;
        ScoreController _scoreController;
        SongObjectExecutionRatingsRecorder _objectRatingRecorder;

        int _maxPossibleScore = 0;

        private void Awake()
        {
            _scoreMesh = this.gameObject.AddComponent<TextMeshPro>();
            _scoreMesh.text = "100.0% - SSS";
            _scoreMesh.fontSize = 3;
            _scoreMesh.color = Color.white;
            _scoreMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            _scoreMesh.rectTransform.position = new Vector3(2.7f, -4.5f, 7f) - new Vector3(_scoreMesh.rectTransform.offsetMin.x, _scoreMesh.rectTransform.offsetMin.y);
            _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
            _objectRatingRecorder = FindObjectOfType<SongObjectExecutionRatingsRecorder>();

            if (_scoreController != null)
            {
                _scoreController.scoreDidChangeEvent += UpdateScore;
            }

        }

        public string GetRank(int score, float prec)
        {
            if (score == _maxPossibleScore)
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
                List<SongObjectExecutionRating> _ratings = ReflectionUtil.GetPrivateField<List<SongObjectExecutionRating>>(_objectRatingRecorder, "_songObjectExecutionRatings");
                if (_ratings != null)
                {
                    int notes = 0;
                    foreach(SongObjectExecutionRating rating in _ratings)
                    {
                        if (rating.songObjectRatingType == SongObjectExecutionRating.SongObjectExecutionRatingType.Note)
                            notes++;
                    }
                    _maxPossibleScore = ScoreController.MaxScoreForNumberOfNotes(notes);
                }
            }

            if (_scoreMesh != null)
            {
                if (_maxPossibleScore == 0)
                    _scoreMesh.text = "100.0% - SSS";
                else
                    _scoreMesh.text = (Mathf.Clamp(percent,0.0f,1.0f) * 100.0f).ToString("F1") + "% - " + GetRank(score, percent);
            }
        }
    } 
}

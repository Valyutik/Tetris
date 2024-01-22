using UnityEngine;
using TMPro;

namespace PlayForge_Team.Tetris.Runtime.UI
{
    public sealed class Score : MonoBehaviour
    {
        private const string BestScoreKey = "BestScore";
        private TMP_Text _scoreText;
        private int _score;
        private int _bestScore;
        
        private void Start()
        {
            FillComponents();
            SetScore(0);
            LoadBestScore();
        }

        private void FillComponents()
        {
            _scoreText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        public void AddScore(int value)
        {
            SetScore(_score + value);
        }

        public void Restart()
        {
            SetScore(0);
        }

        public int GetScore()
        {
            return _score;
        }

        public int GetBestScore()
        {
            return _bestScore;
        }

        public void SetBestScore(int value)
        {
            _bestScore = value;
            SaveBestScore(value);
        }
        
        private void SetScore(int value)
        {
            _score = value;
            SetScoreText(value);
        }

        private void SetScoreText(int value)
        {
            _scoreText.text = $"Очки: {value}";
        }

        private void LoadBestScore()
        {
            _bestScore = PlayerPrefs.GetInt(BestScoreKey);
        }

        private void SaveBestScore(int value)
        {
            PlayerPrefs.SetInt(BestScoreKey, value);
        }
    }
}
using PlayForge_Team.Tetris.Runtime.GameFields;
using PlayForge_Team.Tetris.Runtime.Shapes;
using PlayForge_Team.Tetris.Runtime.UI;
using UnityEngine;
using TMPro;

namespace PlayForge_Team.Tetris.Runtime
{
    public sealed class GameStateChanger : MonoBehaviour
    {
        [SerializeField] private Score score;
        [SerializeField] private TextMeshProUGUI gameEndScoreText;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private GameObject gameScreen;
        [SerializeField] private GameObject gameEndScreen;
        [SerializeField] private ShapeSpawner shapeSpawner;
        [SerializeField] private GameField gameField;
        [SerializeField] private ShapeMover shapeMover;

        private void Start()
        {
            FirstStartGame();
        }
        
        public void SpawnNextShape()
        {
            var nextShape = shapeSpawner.SpawnNextShape();
            shapeMover.SetTargetShape(nextShape);
            shapeMover.MoveShape(Vector2Int.right * (int)(gameField.FieldSize.x * 0.5f) + Vector2Int.up *
                (gameField.FieldSize.y - gameField.InvisibleYFieldSize + nextShape.ExtraSpawnYMove));
        }

        public void RestartGame()
        {
            score.Restart();
            shapeMover.DestroyAllShapes();
            StartGame();
        }
        
        public void EndGame()
        {
            RefreshScores();
            SwitchScreens(false);
            shapeMover.SetActive(false);
        }
        
        private void StartGame()
        {
            SpawnNextShape();
            SwitchScreens(true);
            shapeMover.SetActive(true);
        }

        private void SwitchScreens(bool isGame)
        {
            gameScreen.SetActive(isGame);
            gameEndScreen.SetActive(!isGame);
        }

        private void FirstStartGame()
        {
            gameField.FillCellsPositions();
            StartGame();
        }
        
        private void RefreshScores()
        {
            var value = score.GetScore();
            var oldBestScore = score.GetBestScore();
            var isNewBestScore = CheckNewBestScore(value, oldBestScore);
            SetActiveGameEndScoreText(!isNewBestScore);
            
            if (isNewBestScore)
            {
                score.SetBestScore(value);
                SetNewBestScoreText(value);
            }
            else
            {
                SetGameEndScoreText(value);
                SetOldBestScoreText(oldBestScore);
            }
        }
        
        private bool CheckNewBestScore(int s, int oldBestScore)
        {
            return s > oldBestScore;
        }

        private void SetGameEndScoreText(int value)
        {
            gameEndScoreText.text = $"Игра окончена!\nКоличество очков: {value}";
        }

        private void SetOldBestScoreText(int value)
        {
            bestScoreText.text = $"Лучший результат: {value}";
        }

        private void SetNewBestScoreText(int value)
        {
            bestScoreText.text = $"Новый рекорд: {value}!";
        }

        private void SetActiveGameEndScoreText(bool value)
        {
            gameEndScoreText.gameObject.SetActive(value);
        }
    }
}
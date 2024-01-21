using PlayForge_Team.Tetris.Runtime.GameFields;
using PlayForge_Team.Tetris.Runtime.Shapes;
using UnityEngine;

namespace PlayForge_Team.Tetris.Runtime
{
    public sealed class GameStateChanger : MonoBehaviour
    {
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
            
        }
        
        public void EndGame()
        {
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
    }
}
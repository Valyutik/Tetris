using PlayForge_Team.Tetris.Runtime.GameFields;
using PlayForge_Team.Tetris.Runtime.Shapes;
using UnityEngine;

namespace PlayForge_Team.Tetris.Runtime
{
    public sealed class GameStateChanger : MonoBehaviour
    {
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
            shapeMover.MoveShape(Vector2Int.right * (int)(gameField.FieldSize.x * 0.5f) +
                                 Vector2Int.up * (gameField.FieldSize.y - 3));
        }

        private void FirstStartGame()
        {
            gameField.FillCellsPositions();
            SpawnNextShape();
        }
    }
}
using UnityEngine;

namespace PlayForge_Team.Tetris.Runtime
{
    public sealed class GameField : MonoBehaviour
    {
        [SerializeField] private Transform firstCellPoint;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private Vector2Int fieldSize;
        private GameFieldCell[,] _cells;
        
        public void FillCellsPositions()
        {
            _cells = new GameFieldCell[fieldSize.x, fieldSize.y];

            for (var i = 0; i < fieldSize.x; i++)
            {
                for (var j = 0; j < fieldSize.y; j++)
                {
                    var cellPosition = (Vector2)firstCellPoint.position + Vector2.right * i * cellSize.x + Vector2.up * j * cellSize.y;
                    var newCell = new GameFieldCell(cellPosition);
                    _cells[i, j] = newCell;
                }
            }
        }
        
        public Vector2 GetCellPosition(Vector2Int cellId)
        {
            var cell = GetCell(cellId.x, cellId.y);

            return cell?.GetPosition() ?? Vector2.zero;
        }
        
        private GameFieldCell GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= fieldSize.x || y >= fieldSize.y)
            {
                return null;
            }
            return _cells[x, y];
        }
    }
}
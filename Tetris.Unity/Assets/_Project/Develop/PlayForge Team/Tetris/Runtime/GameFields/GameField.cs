using UnityEngine;

namespace PlayForge_Team.Tetris.Runtime.GameFields
{
    public sealed class GameField : MonoBehaviour
    {
        public int InvisibleYFieldSize => invisibleYFieldSize;
        public Vector2Int FieldSize => fieldSize;
        public Vector2 CellSize => cellSize;
        public Transform FirstCellPoint => firstCellPoint;
        
        [SerializeField] private int invisibleYFieldSize = 4;
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

        private Vector2 GetCellPosition(int x, int y)
        {
            var cell = GetCell(x, y);

            return cell?.GetPosition() ?? Vector2.zero;
        }

        public Vector2Int GetNearestCellId(Vector2 position)
        {
            var resultDistance = float.MaxValue;
            int resultX = 0, resultY = 0;
            
            for (var i = 0; i < FieldSize.x; i++)
            {
                for (var j = 0; j < FieldSize.y; j++)
                {
                    var cellPosition = GetCellPosition(i, j);
                    var distance = (cellPosition - position).magnitude;
                    
                    if (distance < resultDistance)
                    {
                        resultDistance = distance;
                        resultX = i;
                        resultY = j;
                    }
                }
            }
            return new Vector2Int(resultX, resultY);
        }
        
        public void SetCellEmpty(Vector2Int cellId, bool value)
        {
            var cell = GetCell(cellId.x, cellId.y);

            cell?.SetIsEmpty(value);
        }
        
        public bool GetCellEmpty(Vector2Int cellId)
        {
            var cell = GetCell(cellId.x, cellId.y);

            return cell != null && cell.GetIsEmpty();
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
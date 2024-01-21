using PlayForge_Team.Tetris.Runtime.GameFields;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public sealed class ShapeMover : MonoBehaviour
    {
        [SerializeField] private GameStateChanger gameStateChanger;
        [SerializeField] private GameField gameField;
        [SerializeField] private float moveDownDelay = 0.8f;
        private Shape _targetShape;
        private float _moveDownTimer;
        private bool _isActive;

        #region MONO

        private void Update()
        {
            if (!_isActive)
            {
                return;
            }
            
            SetShapePartCellsEmpty(true);
            HorizontalMove();
            VerticalMove();
            Rotate();
            
            var reachBottom = CheckBottom();
            var reachOtherShape = CheckOtherShape();

            SetShapePartCellsEmpty(false);

            if (reachBottom || reachOtherShape)
            {
                if (CheckShapeTopOver())
                {
                    gameStateChanger.EndGame();
                }
                else
                {
                    gameStateChanger.SpawnNextShape();
                }
            }
        }

        #endregion
        
        public void SetActive(bool value)
        {
            _isActive = value;
        }

        public void MoveShape(Vector2Int deltaMove)
        {
            if (!CheckMovePossible(deltaMove))
            {
                return;
            }

            foreach (var shapePart in _targetShape.parts)
            {
                var newPartCellId = shapePart.cellId + deltaMove;
                var newPartPosition = gameField.GetCellPosition(newPartCellId);
                shapePart.cellId = newPartCellId;
                shapePart.SetPosition(newPartPosition);
            }
        }
        
        public void SetTargetShape(Shape targetShape)
        {
            _targetShape = targetShape;
        }
        
        private void Rotate()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                var startCellIds = _targetShape.GetPartCellIds();

                _targetShape.Rotate();
                UpdateByWalls();
                UpdateByBottom();

                var shapeSet = TrySetShapeInCells();

                if (!shapeSet)
                {
                    MoveShapeToCellIds(_targetShape, startCellIds);
                }
            }
        }
        
        private bool CheckShapeTopOver()
        {
            var topCellYPosition = gameField.FirstCellPoint.position.y +
                                   (gameField.FieldSize.y - gameField.InvisibleYFieldSize - 2) * gameField.CellSize.y;

            return _targetShape.parts.Select(t => t.transform.position.y - topCellYPosition)
                .Select(GetRoundedWallDistance)
                .Any(wallDistance => wallDistance != 0 && wallDistance > 0);
        }

        private bool TrySetShapeInCells()
        {
            foreach (var shapePart in _targetShape.parts)
            {
                Vector2 shapePartPosition = shapePart.transform.position;
                var newPartCellId = gameField.GetNearestCellId(shapePartPosition);

                if (!gameField.GetCellEmpty(newPartCellId))
                {
                    return false;
                }
                
                var newPartPosition = gameField.GetCellPosition(newPartCellId);
                shapePart.cellId = newPartCellId;
                shapePart.SetPosition(newPartPosition);
            }
            return true;
        }
        
        private void HorizontalMove()
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                MoveShape(Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                MoveShape(Vector2Int.right);
            }
        }
        
        private bool CheckBottom()
        {
            return _targetShape.parts.Any(t => t.cellId.y == 0);
        }

        private void VerticalMove()
        {
            _moveDownTimer += Time.deltaTime;

            if(_moveDownTimer >= moveDownDelay || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                _moveDownTimer = 0;
                MoveShape(Vector2Int.down);
            }
        }
        
        private bool CheckMovePossible(Vector2Int deltaMove)
        {
            foreach (var t in _targetShape.parts)
            {
                var newPartCellId = t.cellId + deltaMove;

                if (newPartCellId.x < 0 || newPartCellId.y < 0 || newPartCellId.x >= gameField.FieldSize.x ||
                    newPartCellId.y >= gameField.FieldSize.y)
                {
                    return false;
                }

                if (!gameField.GetCellEmpty(newPartCellId))
                {
                    return false;
                }
            }

            return true;
        }
        
        private void UpdateByWalls()
        {
            UpdateByWall(true);
            UpdateByWall(false);
        }

        private void UpdateByWall(bool right)
        {
            foreach (var shapePart in _targetShape.parts)
            {
                if (CheckWallOver(shapePart, right))
                {
                    foreach (var part in _targetShape.parts)
                    {
                        part.transform.position += Vector3.right * ((right ? -1 : 1) * gameField.CellSize.x);
                    }
                }
            }
        }

        private bool CheckWallOver(Component part, bool right)
        {
            float wallDistance;
            if (right)
            {
                wallDistance = part.transform.position.x - (gameField.FirstCellPoint.position.x +
                                                            (gameField.FieldSize.x - 1) * gameField.CellSize.x);
                wallDistance = GetRoundedWallDistance(wallDistance);
                if (wallDistance !=  0 && wallDistance > 0)
                {
                    return true;
                }
            }
            else
            {
                wallDistance = part.transform.position.x - gameField.FirstCellPoint.position.x;
                wallDistance = GetRoundedWallDistance(wallDistance);
                
                if (wallDistance != 0 && wallDistance < 0)
                {
                    return true;
                }
            }
            
            return false;
        }

        private float GetRoundedWallDistance(float distance)
        {
            const int roundValue = 100;
            distance = Mathf.Round(distance * roundValue);
            return distance;
        }
        
        private void UpdateByBottom()
        {
            foreach (var shapePart in _targetShape.parts)
            {
                if (CheckBottomOver(shapePart))
                {
                    foreach (var t in _targetShape.parts)
                    {
                        t.transform.position += Vector3.up * gameField.CellSize.y;
                    }
                }
            }
        }

        private bool CheckBottomOver(Component part)
        {
            var wallDistance = part.transform.position.y - gameField.FirstCellPoint.position.y;
            wallDistance = GetRoundedWallDistance(wallDistance);
            
            return wallDistance != 0 && wallDistance < 0;
        }
        
        private bool CheckOtherShape()
        {
            return _targetShape.parts.Any(t => !gameField.GetCellEmpty(t.cellId + Vector2Int.down));
        }
        
        private void SetShapePartCellsEmpty(bool value)
        {
            foreach (var shapePart in _targetShape.parts)
            {
                gameField.SetCellEmpty(shapePart.cellId, value);
            }
        }
        
        private void MoveShapeToCellIds(Shape shape, IReadOnlyList<Vector2Int> cellIds)
        {
            for (var i = 0; i < shape.parts.Length; i++)
            {
                MoveShapePartToCellId(shape.parts[i], cellIds[i]);
            }
        }
        
        private void MoveShapePartToCellId(ShapePart part, Vector2Int cellId)
        {
            var newPartPosition = gameField.GetCellPosition(cellId);

            part.cellId = cellId;

            part.SetPosition(newPartPosition);
        }
    }
}
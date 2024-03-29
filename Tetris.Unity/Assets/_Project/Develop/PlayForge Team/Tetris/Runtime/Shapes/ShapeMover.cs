﻿using PlayForge_Team.Tetris.Runtime.GameFields;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlayForge_Team.Tetris.Runtime.UI;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public sealed class ShapeMover : MonoBehaviour
    {
        [SerializeField] private float fastDownTimeSpeed = 2f;
        [SerializeField] private Score score;
        [SerializeField] private int scoreByShape = 1;
        [SerializeField] private int scoreByRow = 10;
        [SerializeField] private GameStateChanger gameStateChanger;
        [SerializeField] private GameField gameField;
        [SerializeField] private float moveDownDelay = 0.8f;
        private readonly List<Shape> _allShapes = new();
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

            SetShapePartCellsEmpty(_targetShape, true);

            HorizontalMove();
            VerticalMove();
            Rotate();

            var reachBottom = CheckBottom();
            var reachOtherShape = CheckOtherShape();
            
            SetShapePartCellsEmpty(_targetShape, false);
            
            if (reachBottom || reachOtherShape)
            {
                EndMovement();
            }
        }

        #endregion

        #region Public methods

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
                MoveShapePart(shapePart, deltaMove);
            }
        }
        
        public void DestroyAllShapes()
        {
            for (var i = _allShapes.Count - 1; i >= 0; i--)
            {
                var shape = _allShapes[i];

                SetShapePartCellsEmpty(shape, true);
                DestroyShape(shape);
            }
        }
        
        public void SetTargetShape(Shape targetShape)
        {
            _targetShape = targetShape;

            if (!_allShapes.Contains(targetShape))
            {
                _allShapes.Add(targetShape);
            }
        }

        #endregion
        
        private void DestroyShape(Shape shape)
        {
            _allShapes.Remove(shape);
            Destroy(shape.gameObject);
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
        
        private void SetShapePartCellEmpty(ShapePart part, bool value)
        {
            if (!part.GetActive())
            {
                return;
            }
            gameField.SetCellEmpty(part.cellId, value);
        }
        
        private void MoveShapePart(ShapePart part, Vector2Int deltaMove)
        {
            var newPartCellId = part.cellId + deltaMove;

            MoveShapePartToCellId(part, newPartCellId);
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

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                _moveDownTimer += Time.deltaTime * fastDownTimeSpeed;
            }
            
            if (_moveDownTimer >= moveDownDelay)
            {
                _moveDownTimer = 0;
                MoveShape(Vector2Int.down);
            }
        }
        
        private void EndMovement()
        {
            if (CheckShapeTopOver())
            {
                gameStateChanger.EndGame();
            }
            else
            {
                TryRemoveFilledRows();
                gameStateChanger.SpawnNextShape();
                score.AddScore(scoreByShape);
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
        
        private void SetShapePartCellsEmpty(Shape shape, bool value)
        {
            foreach (var shapePart in shape.parts)
            {
                SetShapePartCellEmpty(shapePart, value);
            }
        }
        
        private void TryRemoveFilledRows()
        {
            var rowFillings = gameField.GetRowFillings();

            for (var i = rowFillings.Length - 1; i >= 0; i--)
            {
                if (rowFillings[i])
                {
                    RemoveRow(i);
                }
            }
        }

        private void RemoveRow(int id)
        {
            for (var i = 0; i < gameField.FieldSize.y - gameField.InvisibleYFieldSize; i++)
            {
                for (var j = 0; j < _allShapes.Count; j++)
                {
                    var shape = _allShapes[j];

                    foreach (var shapePart in shape.parts)
                    {
                        if (shapePart.cellId.y != i || !shapePart.GetActive())
                        {
                            continue;
                        }
                        
                        if (shapePart.cellId.y > id)
                        {
                            SetShapePartCellEmpty(shapePart, true);
                            MoveShapePart(shapePart, Vector2Int.down);
                            SetShapePartCellEmpty(shapePart, false);
                        }
                        else if (shapePart.cellId.y == id)
                        {
                            SetShapePartCellEmpty(shapePart, true);
                            shape.RemovePart(shapePart);
                            
                            if (shape.CheckNeedDestroy())
                            {
                                DestroyShape(shape);
                                j--;
                            }
                        }
                    }
                }
            }
            score.AddScore(scoreByRow);
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
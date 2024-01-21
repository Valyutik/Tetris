using PlayForge_Team.Tetris.Runtime.GameFields;
using UnityEngine;
using System.Linq;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public sealed class ShapeMover : MonoBehaviour
    {
        
        [SerializeField] private GameField gameField;
        [SerializeField] private Shape targetShape;
        [SerializeField] private float moveDownDelay = 0.8f;
        private float _moveDownTimer;

        #region MONO

        private void Update()
        {
            HorizontalMove();
            VerticalMove();
        }

        #endregion

        public void MoveShape(Vector2Int deltaMove)
        {
            if (!CheckMovePossible(deltaMove))
            {
                return;
            }

            foreach (var shapePart in targetShape.parts)
            {
                var newPartCellId = shapePart.cellId + deltaMove;
                var newPartPosition = gameField.GetCellPosition(newPartCellId);
                shapePart.cellId = newPartCellId;
                shapePart.SetPosition(newPartPosition);
            }
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
            return targetShape.parts.Select(t => t.cellId + deltaMove).All(newPartCellId =>
                newPartCellId is { x: >= 0, y: >= 0 } && newPartCellId.x < gameField.FieldSize.x &&
                newPartCellId.y < gameField.FieldSize.y);
        }
    }
}
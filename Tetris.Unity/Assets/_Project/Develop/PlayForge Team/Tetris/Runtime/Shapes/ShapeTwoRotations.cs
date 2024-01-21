using UnityEngine;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public sealed class ShapeTwoRotations : Shape
    {
        private bool _rotated;

        public override void Rotate()
        {
            float rotateMultiplier = _rotated ? -1 : 1;

            Vector2 rotatePosition = parts[0].transform.position;

            foreach (var shapePart in parts)
            {
                shapePart.transform.RotateAround(rotatePosition, Vector3.forward, 90f * rotateMultiplier);
                shapePart.transform.Rotate(Vector3.forward, -90f * rotateMultiplier);
            }
            _rotated = !_rotated;
        }
    }
}
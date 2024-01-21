using UnityEngine;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public sealed class ShapeFourRotations : Shape
    {
        public override void Rotate()
        {
            Vector2 rotatePosition = parts[0].transform.position;

            foreach (var shapePart in parts)
            {
                shapePart.transform.RotateAround(rotatePosition, Vector3.forward, 90f);
                shapePart.transform.Rotate(Vector3.forward, -90f);
            }
        }
    }
}
using UnityEngine;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public sealed class ShapePart : MonoBehaviour
    {
        public Vector2Int cellId;
        
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    }
}
using UnityEngine;

namespace PlayForge_Team.Tetris.Runtime
{
    public sealed class ShapePart : MonoBehaviour
    {
        [SerializeField] private Vector2Int cellId;
        
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
    }
}
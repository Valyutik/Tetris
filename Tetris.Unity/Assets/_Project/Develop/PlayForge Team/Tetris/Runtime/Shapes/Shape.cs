using UnityEngine;
using System;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public class Shape : MonoBehaviour
    {
        public ShapePart[] parts = Array.Empty<ShapePart>();
        [SerializeField] private int extraSpawnYMove;
        
        public virtual void Rotate() { }
        
        public Vector2Int[] GetPartCellIds()
        {
            var startCellIds = new Vector2Int[parts.Length];

            for (var i = 0; i < parts.Length; i++)
            {
                startCellIds[i] = parts[i].cellId;
            }
            return startCellIds;
        }
    }
}
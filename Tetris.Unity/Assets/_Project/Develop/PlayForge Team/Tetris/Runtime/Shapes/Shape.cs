using UnityEngine;
using System.Linq;
using System;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public class Shape : MonoBehaviour
    {
        public int ExtraSpawnYMove => extraSpawnYMove;
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
        
        public void RemovePart(ShapePart part)
        {
            foreach (var shapePart in parts)
            {
                if (shapePart == part)
                {
                    part.SetActive(false);
                }
            }
        }
        
        public bool CheckNeedDestroy()
        {
            return parts.All(t => !t.GetActive());
        }
        
        public bool CheckContainsCellId(Vector2Int cellId)
        {
            return parts.Any(t => t.cellId == cellId);
        }
    }
}
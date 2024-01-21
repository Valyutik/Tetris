using UnityEngine;
using System;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public class Shape : MonoBehaviour
    {
        public ShapePart[] parts = Array.Empty<ShapePart>();
        
        public virtual void Rotate() { }
    }
}
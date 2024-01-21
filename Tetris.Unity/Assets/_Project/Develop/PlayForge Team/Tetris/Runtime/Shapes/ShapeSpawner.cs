using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayForge_Team.Tetris.Runtime.Shapes
{
    public sealed class ShapeSpawner : MonoBehaviour
    {
        [SerializeField] private Shape[] shapePrefabs = Array.Empty<Shape>();

        public Shape SpawnNextShape()
        {
            var randomPrefab = shapePrefabs[Random.Range(0, shapePrefabs.Length)];
            return Instantiate(randomPrefab);
        }
    }
}
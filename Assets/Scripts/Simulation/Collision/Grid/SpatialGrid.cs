using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{
    public class SpatialGrid
    {
        private readonly float _cellSize;
        public float CellSize { get; }
        private readonly Dictionary<GridCell, List<CollisionShape>> _cells;
        const float epsilon = 0.0001f;
        public SpatialGrid(float cellSize)
        {
            _cellSize = cellSize;
            _cells = new Dictionary<GridCell, List<CollisionShape>>();
        }

        private GridCell WorldToCell(Vector3 p)
        {
            return new GridCell
            (
                (int)MathF.Floor(p.x / _cellSize),
                (int)MathF.Floor(p.y / _cellSize),
                (int)MathF.Floor(p.z / _cellSize)
            );
        }


        public IEnumerable<GridCell> GetOverlappingCells(AABB bounds)
        {
            var minCell = WorldToCell(bounds.Min);
            var maxAdjusted = new Vector3(
                bounds.Max.x - epsilon,
                bounds.Max.y - epsilon,
                bounds.Max.z - epsilon
            );
            var maxCell = WorldToCell(maxAdjusted);

            for (int x = minCell.X; x <= maxCell.X; x++)
                for (int y = minCell.Y; y <= maxCell.Y; y++)
                    for (int z = minCell.Z; z <= maxCell.Z; z++)
                        yield return new GridCell(x, y, z);
        }

        public void Insert(CollisionShape shape)
        {
            var cells = GetOverlappingCells(shape.WorldAABB);

            foreach (var cell in cells)
            {
                if (!_cells.TryGetValue(cell, out var list))
                {
                    list = new List<CollisionShape>();
                    _cells[cell] = list;
                }

                list.Add(shape);
                shape.OccupiedCells.Add(cell);
            }   
        }

        public void Remove(CollisionShape shape)
        {
            foreach (var cell in shape.OccupiedCells)
            {
                if (_cells.TryGetValue(cell, out var list))
                {
                    list.Remove(shape);

                    if (list.Count == 0)
                        _cells.Remove(cell);
                }
            }

            shape.OccupiedCells.Clear();
        }

        public IEnumerable<GridCell> GetCells(AABB bounds)
        {
            const float epsilon = 0.0001f;

            var min = bounds.Min;
            var max = bounds.Max;

            int minX = (int)MathF.Floor(min.x / _cellSize);
            int minY = (int)MathF.Floor(min.y / _cellSize);
            int minZ = (int)MathF.Floor(min.z / _cellSize);

            int maxX = (int)MathF.Floor((max.x - epsilon) / _cellSize);
            int maxY = (int)MathF.Floor((max.y - epsilon) / _cellSize);
            int maxZ = (int)MathF.Floor((max.z - epsilon) / _cellSize);

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    for (int z = minZ; z <= maxZ; z++)
                        yield return new GridCell(x, y, z);
        }

        public bool TryGetCell(GridCell cell, out List<CollisionShape> shapes)
        {
            return _cells.TryGetValue(cell, out shapes);
        }

    }
}
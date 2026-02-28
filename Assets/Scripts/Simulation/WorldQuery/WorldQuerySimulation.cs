using System.Collections.Generic;
using UnityEngine;
namespace Simulator
{
    public class WorldQuerySimulation
    {
        
        private readonly SpatialGrid _staticGrid;
        private readonly SpatialGrid _dynamicGrid;

        private int _queryVersion;

        public WorldQuerySimulation(
            SpatialGrid staticGrid,
            SpatialGrid dynamicGrid)
        {
            _staticGrid = staticGrid;
            _dynamicGrid = dynamicGrid;
        }

        public void QueryAABB(
            AABB query,
            int layerMask,
            List<CollisionShape> results,
            bool includeStatic = true,
            bool includeDynamic = true)
        {
            results.Clear();
            int version = ++_queryVersion;

            if (includeStatic)
                QueryGrid(_staticGrid, query, layerMask, results, version);

            if (includeDynamic)
                QueryGrid(_dynamicGrid, query, layerMask, results, version);
        }

        public bool OverlapAABB(
            AABB query,
            int layerMask,
            bool includeStatic = true,
            bool includeDynamic = true)
        {
            int version = ++_queryVersion;

            if (includeStatic &&
                OverlapGrid(_staticGrid, query, layerMask, version))
                return true;

            if (includeDynamic &&
                OverlapGrid(_dynamicGrid, query, layerMask, version))
                return true;

            return false;
        }

        private void QueryGrid(
            SpatialGrid grid,
            AABB query,
            int layerMask,
            List<CollisionShape> results,
            int version)
        {
            foreach (var cell in grid.GetOverlappingCells(query))
            {
                if (!grid.TryGetCell(cell, out var shapes))
                    continue;

                foreach (var shape in shapes)
                {
                    if (shape.LastQueryVersion == version)
                        continue;

                    shape.LastQueryVersion = version;

                    if (!LayerMatches(shape.Layer, layerMask))
                        continue;

                    if (!shape.WorldAABB.Intersects(query))
                        continue;

                    results.Add(shape);
                }
            }
        }

        private bool OverlapGrid(
            SpatialGrid grid,
            AABB query,
            int layerMask,
            int version)
        {
            foreach (var cell in grid.GetOverlappingCells(query))
            {
                if (!grid.TryGetCell(cell, out var shapes))
                    continue;

                foreach (var shape in shapes)
                {
                    if (shape.LastQueryVersion == version)
                        continue;

                    shape.LastQueryVersion = version;

                    if (!LayerMatches(shape.Layer, layerMask))
                        continue;

                    if (shape.WorldAABB.Intersects(query))
                        return true;
                }
            }

            return false;
        }

        private static bool LayerMatches(CollisionLayer layer, int mask)
        {
            int bit = 1 << (int)layer;
            return (mask & bit) != 0;
        }
    }
}
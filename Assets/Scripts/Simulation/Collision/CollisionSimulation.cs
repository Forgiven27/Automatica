using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{
    public class CollisionSimulation
    {
        private readonly SpatialGrid _staticGrid;
        private readonly SpatialGrid _dynamicGrid;
        public SpatialGrid StaticGrid { get { return _staticGrid; } }
        public SpatialGrid DynamicGrid { get { return _dynamicGrid; } }

        private readonly Dictionary<uint, CollisionObject> _objects;
        private int _queryVersion;
        private EntityRegistry _entityRegistry;

        public CollisionSimulation(CollisionConfig config, EntityRegistry entityRegistry)
        {
            _staticGrid = new SpatialGrid(config.CellSize);
            _dynamicGrid = new SpatialGrid(config.CellSize);
            _objects = new Dictionary<uint, CollisionObject>();
            _entityRegistry = entityRegistry;
            _entityRegistry.EntityRemoved += OnEntityRemoved;
        }

        public Dictionary<uint, CollisionObject> GetObjects() { return _objects; }

        public void UpdateTransform(uint ownerId, TransformSim transform)
        {

            if (!_objects.TryGetValue(ownerId, out var obj))
                return;

            if (!obj.IsDynamic)
                return;

            foreach (var shape in obj.Shapes)
            {
                _dynamicGrid.Remove(shape);

                shape.WorldAABB = CollisionShape.CalculateWorldAABB(
                    shape.LocalAABB,
                    transform);

                _dynamicGrid.Insert(shape);
            }
        }


        public void RegisterStaticObject(CollisionObject obj)
        {
            _objects[obj.ID] = obj;

            foreach (var shape in obj.Shapes)
                _staticGrid.Insert(shape);
        }
        public void RegisterDynamicObject(CollisionObject obj)
        {
            _objects[obj.ID] = obj;

            foreach (var shape in obj.Shapes)
                _dynamicGrid.Insert(shape);
        }
        public uint RegisterStaticObjectWoutID(CollisionObject obj)
        {
            uint id = IDHandler.GetID();
            obj.ID = id;
            _objects[obj.ID] = obj;

            foreach (var shape in obj.Shapes)
            {
                shape.OwnerId = id;
                _staticGrid.Insert(shape);
            }
            return id;
        }

        private void OnEntityRemoved(uint ID)
        {
            if (!_objects.TryGetValue(ID, out var obj))
                return;
            if (obj.IsDynamic)
            {
                UnregisterDynamicObject(ID);
            }
            else
            {
                UnregisterStaticObject(ID);
            }

        }

        public void Overlap(AABB bounds, CollisionLayer mask, List<CollisionShape> results, uint ignoreOwner = 0)
        {
            _queryVersion++;

            QueryGrid(_staticGrid);
            QueryGrid(_dynamicGrid);

            void QueryGrid(SpatialGrid grid)
            {
                foreach (var cell in grid.GetCells(bounds))
                {
                    if (!grid.TryGetCell(cell, out var list))
                        continue;

                    foreach (var shape in list)
                    {
                        if ((shape.Layer & mask) == 0)
                            continue;

                        if (ignoreOwner != 0 && shape.OwnerId == ignoreOwner)
                            continue;

                        if (shape.LastQueryVersion == _queryVersion)
                            continue;

                        shape.LastQueryVersion = _queryVersion;

                        if (AABBIntersect(bounds, shape.WorldAABB))
                            results.Add(shape);
                    }
                }
            }
        }

        private static bool AABBIntersect(in AABB a, in AABB b)
        {
            if (a.Max.x <= b.Min.x || a.Min.x >= b.Max.x) return false;
            if (a.Max.y <= b.Min.y || a.Min.y >= b.Max.y) return false;
            if (a.Max.z <= b.Min.z || a.Min.z >= b.Max.z) return false;

            return true;
        }

        public bool TryMove(CollisionShape shape,AABB newBounds,CollisionLayer mask)
        {
            var oldBounds = shape.WorldAABB;

            shape.WorldAABB = newBounds;

            if (HasBlocking(newBounds, mask, self: shape))
            {
                shape.WorldAABB = oldBounds;
                return false;
            }

            _dynamicGrid.Remove(shape);
            _dynamicGrid.Insert(shape);

            return true;
        }

        public bool HasBlocking(
        AABB bounds,
        CollisionLayer mask,
        CollisionShape self = null,
        uint ignoreOwner = 0)
        {
            _queryVersion++;

            if (CheckGrid(_staticGrid)) return true;
            if (CheckGrid(_dynamicGrid)) return true;

            return false;

            bool CheckGrid(SpatialGrid grid)
            {
                foreach (var cell in grid.GetCells(bounds))
                {
                    if (!grid.TryGetCell(cell, out var list))
                        continue;

                    foreach (var shape in list)
                    {
                        if ((shape.Layer & mask) == 0)
                            continue;

                        if (shape == self)
                            continue;

                        if (ignoreOwner != 0 && shape.OwnerId == ignoreOwner)
                            continue;

                        if (shape.LastQueryVersion == _queryVersion)
                            continue;

                        shape.LastQueryVersion = _queryVersion;

                        if (AABBIntersect(bounds, shape.WorldAABB))
                            return true;
                    }
                }

                return false;
            }
        }
        public bool TryRelease(CollisionShape shape, AABB releaseBounds, CollisionLayer mask)
        {
            shape.WorldAABB = releaseBounds;

            if (HasBlocking(releaseBounds, mask))
                return false;

            _dynamicGrid.Insert(shape);
            return true;
        }

        public bool Raycast(
            Vector3 origin,
            Vector3 direction,
            float maxDistance,
            CollisionLayer mask,
            out CollisionShape hitShape,
            out float hitDistance,
            uint ignoreOwner = 0)
        {
            hitShape = null;
            hitDistance = 0f;

            if (direction.sqrMagnitude == 0f)
                return false;

            direction.Normalize();

            _queryVersion++;

            float cellSize = _staticGrid.CellSize; 

            int x = (int)MathF.Floor(origin.x / cellSize);
            int y = (int)MathF.Floor(origin.y / cellSize);
            int z = (int)MathF.Floor(origin.z / cellSize);

            int stepX = direction.x > 0 ? 1 : (direction.x < 0 ? -1 : 0);
            int stepY = direction.y > 0 ? 1 : (direction.y < 0 ? -1 : 0);
            int stepZ = direction.z > 0 ? 1 : (direction.z < 0 ? -1 : 0);

            float nextBoundaryX = stepX > 0
                ? (x + 1) * cellSize
                : x * cellSize;

            float nextBoundaryY = stepY > 0
                ? (y + 1) * cellSize
                : y * cellSize;

            float nextBoundaryZ = stepZ > 0
                ? (z + 1) * cellSize
                : z * cellSize;

            float tMaxX = stepX != 0
                ? (nextBoundaryX - origin.x) / direction.x
                : float.PositiveInfinity;

            float tMaxY = stepY != 0
                ? (nextBoundaryY - origin.y) / direction.y
                : float.PositiveInfinity;

            float tMaxZ = stepZ != 0
                ? (nextBoundaryZ - origin.z) / direction.z
                : float.PositiveInfinity;

            float tDeltaX = stepX != 0
                ? cellSize / MathF.Abs(direction.x)
                : float.PositiveInfinity;

            float tDeltaY = stepY != 0
                ? cellSize / MathF.Abs(direction.y)
                : float.PositiveInfinity;

            float tDeltaZ = stepZ != 0
                ? cellSize / MathF.Abs(direction.z)
                : float.PositiveInfinity;

            float t = 0f;

            while (t <= maxDistance)
            {
                var cell = new GridCell(x, y, z);

                if (CheckCell(_staticGrid, cell, origin, direction, maxDistance, mask, ignoreOwner, ref hitShape, ref hitDistance))
                    return true;

                if (CheckCell(_dynamicGrid, cell, origin, direction, maxDistance, mask, ignoreOwner, ref hitShape, ref hitDistance))
                    return true;

                if (tMaxX < tMaxY)
                {
                    if (tMaxX < tMaxZ)
                    {
                        x += stepX;
                        t = tMaxX;
                        tMaxX += tDeltaX;
                    }
                    else
                    {
                        z += stepZ;
                        t = tMaxZ;
                        tMaxZ += tDeltaZ;
                    }
                }
                else
                {
                    if (tMaxY < tMaxZ)
                    {
                        y += stepY;
                        t = tMaxY;
                        tMaxY += tDeltaY;
                    }
                    else
                    {
                        z += stepZ;
                        t = tMaxZ;
                        tMaxZ += tDeltaZ;
                    }
                }
            }

            return false;
        }


        private bool CheckCell(
            SpatialGrid grid,
            GridCell cell,
            Vector3 origin,
            Vector3 direction,
            float maxDistance,
            CollisionLayer mask,
            uint ignoreOwner,
            ref CollisionShape hitShape,
            ref float hitDistance)
        {
            if (!grid.TryGetCell(cell, out var list))
                return false;

            foreach (var shape in list)
            {
                if ((shape.Layer & mask) == 0)
                    continue;

                if (ignoreOwner != 0 && shape.OwnerId == ignoreOwner)
                    continue;

                if (shape.LastQueryVersion == _queryVersion)
                    continue;

                shape.LastQueryVersion = _queryVersion;

                if (RayAABB(origin, direction, shape.WorldAABB, out float dist))
                {
                    if (dist <= maxDistance)
                    {
                        hitShape = shape;
                        hitDistance = dist;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool RayAABB(Vector3 origin, Vector3 direction, in AABB box,out float distance)
        {
            float tMin = 0f;
            float tMax = float.MaxValue;

            if (!Slab(origin.x, direction.x, box.Min.x, box.Max.x, ref tMin, ref tMax) ||
                !Slab(origin.y, direction.y, box.Min.y, box.Max.y, ref tMin, ref tMax) ||
                !Slab(origin.z, direction.z, box.Min.z, box.Max.z, ref tMin, ref tMax))
            {
                distance = 0f;
                return false;
            }

            distance = tMin;
            return true;
        }

        private static bool Slab(
            float origin,
            float dir,
            float min,
            float max,
            ref float tMin,
            ref float tMax)
        {
            if (MathF.Abs(dir) < 1e-6f)
            {
                return origin >= min && origin <= max;
            }

            float inv = 1f / dir;
            float t1 = (min - origin) * inv;
            float t2 = (max - origin) * inv;

            if (t1 > t2)
                (t1, t2) = (t2, t1);

            tMin = MathF.Max(tMin, t1);
            tMax = MathF.Min(tMax, t2);

            return tMin <= tMax;
        }



        public void UnregisterStaticObject(uint ID)
        {
            if (!_objects.TryGetValue(ID, out var obj))
                return;

            foreach (var shape in obj.Shapes)
                _staticGrid.Remove(shape);

            _objects.Remove(ID);
        }
        public void UnregisterDynamicObject(uint id)
        {
            if (!_objects.TryGetValue(id, out var obj))
                return;

            foreach (var shape in obj.Shapes)
                _dynamicGrid.Remove(shape);

            _objects.Remove(id);
        }
        public void UpdateDynamicShape(CollisionShape shape)
        {
            _dynamicGrid.Remove(shape);
            _dynamicGrid.Insert(shape);
        }
    }

    [Flags]
    public enum CollisionLayer
    {
        StaticWorld = 1,
        Conveyor = 2,
        Storage = 4,
        Manipulator = 8,
        ItemInteractionZone = 16
    }

    public enum ShapeType
    {
        AABB,
        Capsule
    }
}
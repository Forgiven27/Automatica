using System.Collections.Generic;
using UnityEngine;

namespace Simulator 
{ 
    public abstract class CollisionShape
    {
        public uint OwnerId { get; internal set; }
        public CollisionLayer Layer { get; internal set; }
        public AABB WorldAABB { get; internal set; }
        public AABB LocalAABB { get; internal set; }


        internal List<GridCell> OccupiedCells = new();
        internal int LastQueryVersion;

        public static AABB CalculateWorldAABB(AABB local, TransformSim transform)
        {
            Matrix4x4 matrix =
                Matrix4x4.TRS(transform.position, transform.rotation, transform.scale);

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Vector3[] corners = new Vector3[8];

            corners[0] = new(local.Min.x, local.Min.y, local.Min.z);
            corners[1] = new(local.Max.x, local.Min.y, local.Min.z);
            corners[2] = new(local.Min.x, local.Max.y, local.Min.z);
            corners[3] = new(local.Max.x, local.Max.y, local.Min.z);
            corners[4] = new(local.Min.x, local.Min.y, local.Max.z);
            corners[5] = new(local.Max.x, local.Min.y, local.Max.z);
            corners[6] = new(local.Min.x, local.Max.y, local.Max.z);
            corners[7] = new(local.Max.x, local.Max.y, local.Max.z);

            for (int i = 0; i < 8; i++)
            {
                Vector3 world = matrix.MultiplyPoint3x4(corners[i]);

                min = Vector3.Min(min, world);
                max = Vector3.Max(max, world);
            }

            return new AABB(min, max);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
namespace Simulator
{
    public static class ShapeUpdater
    {
        public static void RecalculateShapes(
            CollisionSimulation collision,
            TransformSim baseTransform,
            IReadOnlyList<TransformSim> boneWorldTransforms,
            List<ShapeBinding> bindings)
        {
            foreach (var binding in bindings)
            {
                TransformSim parent;

                if (binding.RelatedJointIndex < 0)
                    parent = baseTransform;
                else
                    parent = boneWorldTransforms[binding.RelatedJointIndex];

                TransformSim world =
                    TransformSim.Combine(parent, binding.LocalTransform);

                var shape = binding.Shape;

                shape.WorldAABB = CollisionShape.CalculateWorldAABB(
                    shape.LocalAABB,
                    world);

                collision.UpdateDynamicShape(shape);
            }
        }
    }
}
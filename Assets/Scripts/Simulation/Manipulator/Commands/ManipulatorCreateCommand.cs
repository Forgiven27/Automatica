using Simulator;
using UnityEngine;
namespace Simulator
{
    public struct ManipulatorCreateCommand : ISimulatorCommand
    {
        public Bone[] bones;
        public float baseYaw;

        public CollisionObject collisionObject;
        public TransformSim transform;

        public void Execute(CommandContext context)
        {
            context.manipulatorSim.Create(this, context.simulation);
            context.collisionSim.RegisterDynamicObject(collisionObject);
            context.entities.Add(collisionObject, transform);
        }
    }
}
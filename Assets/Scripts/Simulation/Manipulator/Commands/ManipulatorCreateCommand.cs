using Simulator;
using UnityEngine;
namespace Simulator
{
    public struct ManipulatorCreateCommand : ISimulatorCommand
    {
        public LogicBone[] logicBones;
        public KinematicBone[] kinematicBones;
        public ShapeBinding[] shapeBindings;

        public float baseYaw;

        public CollisionObject collisionObject;
        public TransformSim baseTransform;

        public ManipulatorCreateCommand(float baseYaw, TransformSim baseTransform,
            CollisionObject collisionObject, LogicBone[] logicBones, KinematicBone[] kinematicBones,
            ShapeBinding[] shapeBindings)
        {
            this.baseYaw = baseYaw;
            this.baseTransform = baseTransform;
            this.collisionObject = collisionObject;
            this.logicBones = logicBones;
            this.kinematicBones = kinematicBones;
            this.shapeBindings = shapeBindings;
        }

        public void Execute(CommandContext context)
        {
            uint id = context.manipulatorSim.Create(this, context.simulation);
            
            if (collisionObject.ID == 0)
            {
                collisionObject.ID = id;
                collisionObject.Shapes.ForEach(x => x.OwnerId = id);
            }

            context.collisionSim.RegisterDynamicObject(collisionObject);
            context.entities.Add(collisionObject, baseTransform);
        }
    }
}
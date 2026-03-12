using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{

    public class ManipulatorRuntime :IEntity
    {
        public uint ID {  get; set; }

        public ManipulatorLogic Logic;
        public ManipulatorKinematics Kinematics;

        public TransformSim BaseTransform;

        public List<ShapeBinding> Shapes;

        public ManipulatorRuntime(uint id, 
            ManipulatorLogic manipulatorLogic, 
            ManipulatorKinematics manipulatorKinematics,
            TransformSim baseTranform,
            List<ShapeBinding> shapeBindings)
        {
            ID = id;
            Logic = manipulatorLogic;
            Kinematics = manipulatorKinematics;
            BaseTransform = baseTranform;
            Shapes = shapeBindings;

            
        }


        public void Tick(ManipulatorContext context)
        {
            Logic.Work(context, this);
            Kinematics.Update(BaseTransform.position, Logic.BaseYaw, Logic.BoneSnapshot);
            ShapeUpdater.RecalculateShapes(
                context.collision,
                BaseTransform,
                Kinematics.WorldTransforms,
                Shapes);
        }


        public void CreateGrabOperation(ManipulatorContext context)
        {
            List<CollisionShape> overlapedShapes = new();
            context.collision.Overlap(Shapes[Shapes.Count - 1].Shape.WorldAABB,
                CollisionLayer.ItemInteractionZone, overlapedShapes, ID);
            if ( overlapedShapes.Count == 0 )
            {
                Debug.Log("ManipulatorGrab: Нет объектов для взаимодействия");
                return;
            }
            context.operations.Add(new SimOperation()
            {
                entityOpAuthor = new PortRef() { entityId = ID },
                type = SimOpType.GrabItem,
                entityOpAnother = new PortRef() { entityId = overlapedShapes[0].OwnerId },
            });
        }

        public void CreateReleaseOperation(ManipulatorContext context)
        {
            List<CollisionShape> overlapedShapes = new();
            context.collision.Overlap(Shapes[Shapes.Count - 1].Shape.WorldAABB,
                CollisionLayer.ItemInteractionZone, overlapedShapes, ID);
            context.collision.Overlap(Shapes[Shapes.Count - 1].Shape.WorldAABB,
                CollisionLayer.Storage, overlapedShapes, ID);


            context.operations.Add(new SimOperation()
            {
                entityOpAuthor = new PortRef() { entityId = ID },
                type = SimOpType.ReleaseItem,
                entityOpAnother = new PortRef() { entityId = overlapedShapes[0].OwnerId },
            });
        }


    }
}
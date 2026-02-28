using UnityEngine;
namespace Simulator
{
    public struct ConveyorCreateCommand : ISimulatorCommand
    {
        public TransformSim[] segmentsTransform;
        /*
        public Vector3 startPosition;
        public Vector3 endPosition;
        */
        public void Execute(CommandContext context)
        {
            IEntity entity = context.conveyorSim.Create(this, context.simulation);
            context.entities.Add(entity);
        }
    }
}

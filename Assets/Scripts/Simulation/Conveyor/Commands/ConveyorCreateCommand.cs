
namespace Simulator
{
    public struct ConveyorCreateCommand : ISimulatorCommand
    {
        public TransformSim[] segmentsTransform;
        public CollisionObject[] segmentsCollsion;
        public TransformSim lineTransform;

        public ConveyorCreateCommand(TransformSim[] segmentsTransform, CollisionObject[] segmentsCollision, TransformSim lineTransform)
        {
            this.segmentsTransform = segmentsTransform;
            this.segmentsCollsion = segmentsCollision;
            this.lineTransform = lineTransform;
        }

        public void Execute(CommandContext context)
        {
            ConveyorLine line = context.conveyorSim.Create(this, context.simulation);
            foreach (var segment in segmentsCollsion)
            {
                context.collisionSim.RegisterDynamicObject(segment);
            }
            context.entities.Add(line, lineTransform);
            for (int i = 0; i < line.segments.Count; i++)
            {
                context.entities.Add(line.segments[i], segmentsTransform[i]);   
            }
        }
    }
}

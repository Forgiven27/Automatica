
namespace Simulator
{
    public struct ConveyorCreateCommand : ISimulatorCommand
    {
        public TransformSim[] segmentsTransform;
        public CollisionObject[] segmentsCollsion; // TODO
        public TransformSim lineTranform;
        public void Execute(CommandContext context)
        {
            ConveyorLine line = context.conveyorSim.Create(this, context.simulation);
            context.entities.Add(line, lineTranform);
            foreach (var segment in line.segments)
            {    
                context.entities.Add(segment, lineTranform);   
            }
        }
    }
}

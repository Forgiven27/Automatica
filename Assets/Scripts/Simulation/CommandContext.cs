namespace Simulator
{
    public class CommandContext
    {
        public Simulation simulation;
        public FactorySimulation factorySim;
        public ConveyorSimulation conveyorSim;
        public EntityRegistry entities;
        public ConnectionSystem connections;
    }
}
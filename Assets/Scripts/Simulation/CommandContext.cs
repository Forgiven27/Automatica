namespace Simulator
{
    public class CommandContext
    {
        public Simulation simulation;
        public FactorySimulation factorySim;
        public ConveyorSimulation conveyorSim;
        public ManipulatorSimulation manipulatorSim;
        public CollisionSimulation collisionSim;
        public EntityRegistry entities;
        public ConnectionSystem connections;
        public WorldQuerySimulation worldQuery;

        public CommandContext(Simulation simulation,
            FactorySimulation factorySim,
            ConveyorSimulation conveyorSim,
            ManipulatorSimulation manipulator,
            CollisionSimulation collisionSim,
            EntityRegistry entities,
            ConnectionSystem connections,
            WorldQuerySimulation worldQuery)
        {
            this.simulation = simulation;
            this.factorySim = factorySim;
            this.conveyorSim = conveyorSim;
            this.manipulatorSim = manipulator;
            this.collisionSim = collisionSim;
            this.entities = entities;
            this.connections = connections;
            this.worldQuery = worldQuery;
        }
    }
}
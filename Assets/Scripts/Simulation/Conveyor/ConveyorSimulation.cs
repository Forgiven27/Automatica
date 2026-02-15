using System.Collections.Generic;



namespace Simulator { 
    public class ConveyorSimulation
    {
        List<ConveyorLine> lines = new();


        public void Tick()
        {
            foreach (var line in lines) {
                line.Work();
            }
        }

        public ConveyorLine Create(ConveyorCreateCommand cmd, Simulation sim)
        {
            string genID = System.Guid.NewGuid().ToString();
            var conveyor = new ConveyorLine(genID, cmd.stepsOfContainer);
            lines.Add(conveyor);

            sim.Events.Raise(new ConveyorCreatedEvent()
            {
                conveyorID = genID,
                startPosition = cmd.startPosition,
                endPosition = cmd.endPosition,
            });
            return conveyor;
        }


        public ConveyorSnapshot GetSnapshotById(string id)
        {
            ConveyorLine line = lines.Find(line => line.ID == id);
            var items = line.GetItems();
            return new ConveyorSnapshot()
            {
                items = items
            }; 
        }
    }
}
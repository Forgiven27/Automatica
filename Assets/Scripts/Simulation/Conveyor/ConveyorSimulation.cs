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
            uint lineID = IDHandler.GetID();
            uint[] segmentsID = new uint[cmd.segmentsTransform.Length];
            for (int i = 0; i < segmentsID.Length; i++)
            {
                segmentsID[i] = IDHandler.GetID();
            }
            var conveyor = new ConveyorLine(lineID, segmentsID);
            lines.Add(conveyor);

            sim.Events.Raise(new ConveyorCreatedEvent()
            {
                conveyorID = lineID,
                segmentsID = segmentsID,
                segmentsTransform = cmd.segmentsTransform,
            });
            return conveyor;
        }


        public ConveyorSnapshot GetSnapshotById(uint id)
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
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Simulator { 
    public class FactorySimulation
    {
        List<Factory> factories = new();


        public Factory Create(FactoryCreateCommand cmd, Simulation sim)
        {
            uint genID = IDHandler.GetID();
            var factory = new Factory(
                genID,
                new FactoryGenerator(cmd.factoryDescription.generator),
                cmd.factoryDescription.slots.DeepCopyList(),
                cmd.factoryDescription.ports.DeepCopyList()
            );
            factories.Add(factory);

            sim.Events.Raise(new FactoryCreatedEvent()
            {
                factoryId = genID,
                factoryType = cmd.factoryType,
                position = cmd.position,
                rotation = cmd.rotation,
            });

            return factory;
        }

    


        public void Delete(uint id)
        {
            foreach (Factory factory in factories)
            {
                if (factory.ID == id)
                {
                    factories.Remove(factory);
                }
            }
        }
        public void Delete(Factory factoryToDelete)
        {
            foreach (Factory factory in factories)
            {
                if (factory.ID == factoryToDelete.ID)
                {
                    factories.Remove(factory);
                }
            }
        }
        public void Tick(Simulation simulation)
        {
            foreach(Factory factory in factories)
            {
                FactoryContext context = simulation.CreateFactoryContext(factory.ID);
                factory.Work(context);
            }
        }

        public FactorySnapshot GetSnapshotById(uint id)
        {
            Factory factory = factories.Find(factory => factory.ID == id);
            var ports = factory.ports.ToList();
            var slots = factory.slots.ToList();
            var snapshot = new FactorySnapshot()
            {
                ports = ports,
                slots = slots,
            };
            return snapshot;
        }
        

    }

}

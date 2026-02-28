using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Simulator
{
    public class ManipulatorSimulation
    {
        List<Manipulator> manipulators = new List<Manipulator>();

        public Manipulator Create(ManipulatorCreateCommand cmd, Simulation sim)
        {
            string genID = System.Guid.NewGuid().ToString();
            var manipulator = new Manipulator();
            manipulators.Add(manipulator);

            sim.Events.Raise(new FactoryCreatedEvent()
            {

            });

            return manipulator;
        }



        public void Delete(uint id)
        {
            foreach (Manipulator manipulator in manipulators)
            {
                if (manipulator.ID == id)
                {
                    manipulators.Remove(manipulator);
                }
            }
        }
        public void Delete(Manipulator factoryToDelete)
        {
            foreach (Manipulator manipulator in manipulators)
            {
                if (manipulator.ID == factoryToDelete.ID)
                {
                    manipulators.Remove(manipulator);
                }
            }
        }
        public void Tick(Simulation simulation)
        {
            foreach (Manipulator manipulator in manipulators)
            {
                ManipulatorContext context = simulation.CreateManipulatorContext();
                manipulator.Work(context);
            }
        }

        public ManipulatorSnapshot GetSnapshotById(uint id)
        {
            Manipulator manipulator = manipulators.Find(manip => manip.ID == id);
            
            var snapshot = new ManipulatorSnapshot()
            {
            };
            return snapshot;
        }
    }
}
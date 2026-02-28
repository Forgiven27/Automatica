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
            
            var manipulator = new Manipulator(cmd.baseYaw, cmd.bones);
            manipulators.Add(manipulator);

            sim.Events.Raise(new ManipulatorCreatedEvent()
            {
                bones = cmd.bones,
                transform = cmd.transform,
            });

            return manipulator;
        }


        public void Tick(Simulation simulation)
        {
            foreach (Manipulator manipulator in manipulators)
            {
                ManipulatorContext context = simulation.CreateManipulatorContext();
                manipulator.Work(context);
            }
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
        

        public ManipulatorSnapshot GetSnapshotById(uint id)
        {
            Manipulator manipulator = manipulators.Find(manip => manip.ID == id);
            
            var snapshot = new ManipulatorSnapshot()
            {
                baseYaw = manipulator.BaseYaw,
                bones = manipulator.GetBonesSnapshot(),
            };
            return snapshot;
        }
    }
}
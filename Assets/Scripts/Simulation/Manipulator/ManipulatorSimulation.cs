using System.Collections.Generic;
using System.Diagnostics.Tracing;
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
            manipulator.ID = IDHandler.GetID();
            manipulators.Add(manipulator);



            sim.Events.Raise(new ManipulatorCreatedEvent()
            {
                bones = manipulator.GetBonesSnapshot(),
                transform = cmd.transform,
                baseYaw = cmd.baseYaw,
                ID = manipulator.ID,
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


        public void SetScript(uint id, Queue<ManipulatorCommandBuffer> scriptConverted, string scriptText)
        {
            Manipulator manipulator = manipulators.Find(manip => manip.ID == id);
            if (manipulator == null) Debug.LogError($"Манипулятор с ID = {id} не был найден");
            manipulator.ScriptText = scriptText;
            manipulator.SetCommands(scriptConverted);
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

        public uint[] GetAllID()
        {
            uint[] ids = new uint[manipulators.Count];
            
            for(int i = 0; i < ids.Length; i++)
            {
                ids[i] = manipulators[i].ID;
            }

            return ids;
        }

        public string GetScriptTextByID(uint id)
        {
            Manipulator manipulator = manipulators.Find(manip => manip.ID == id);

            return manipulator.ScriptText;
        }
    }
}
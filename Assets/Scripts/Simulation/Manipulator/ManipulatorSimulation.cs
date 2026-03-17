using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Simulator
{
    public class ManipulatorSimulation
    {
        List<ManipulatorRuntime> manipulatorsRuntime = new List<ManipulatorRuntime>();
        


        public uint Create(ManipulatorCreateCommand cmd, Simulation sim)
        {
            var manipultorKinematic = new ManipulatorKinematics(cmd.kinematicBones);
            var manipulatorLogic = new ManipulatorLogic(cmd.baseYaw, cmd.logicBones);

            uint ID = IDHandler.GetID();

            
            ManipulatorRuntime manipulatorRuntime = new(ID, manipulatorLogic, manipultorKinematic,
                cmd.baseTransform, cmd.shapeBindings.ToList());
            manipulatorsRuntime.Add(manipulatorRuntime);

            sim.Events.Raise(new ManipulatorCreatedEvent()
            {
                bones = manipulatorLogic.BoneSnapshot,
                transform = cmd.baseTransform,
                baseYaw = cmd.baseYaw,
                ID = ID,
            });

            return ID;
        }


        public void Tick(Simulation simulation)
        {
            foreach (var runtime in manipulatorsRuntime)
            {
                ManipulatorContext context = simulation.CreateManipulatorContext();
                runtime.Tick(context);
            }
        }


        public void SetScript(uint id, Queue<ManipulatorCommandBuffer> scriptConverted, string scriptText)
        {
            ManipulatorLogic logicManipulator = manipulatorsRuntime.Find(manip => manip.ID == id).Logic;
            if (logicManipulator == null) Debug.LogError($"Манипулятор с ID = {id} не был найден");
            logicManipulator.ScriptText = scriptText;
            logicManipulator.SetCommands(scriptConverted);
        }

        public void Delete(uint id)
        {
            foreach (var runtime in manipulatorsRuntime)
            {
                if (runtime.ID == id)
                {
                    manipulatorsRuntime.Remove(runtime);
                }
            }
        }
        

        public ManipulatorSnapshot GetSnapshotById(uint id)
        {
            ManipulatorLogic manipulator = manipulatorsRuntime.Find(manip => manip.ID == id).Logic;
            
            var snapshot = new ManipulatorSnapshot(
                manipulator.BaseYaw, 
                manipulator.BoneSnapshot,
                manipulator.HeldItem);
            return snapshot;
        }

        public uint[] GetAllID()
        {
            uint[] ids = new uint[manipulatorsRuntime.Count];
            
            for(int i = 0; i < ids.Length; i++)
            {
                ids[i] = manipulatorsRuntime[i].ID;
            }

            return ids;
        }

        public string GetScriptTextByID(uint id)
        {
            ManipulatorLogic manipulator = manipulatorsRuntime.Find(manip => manip.ID == id).Logic;

            return manipulator.ScriptText;
        }
    }
}
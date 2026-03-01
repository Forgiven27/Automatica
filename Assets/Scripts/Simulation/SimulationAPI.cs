using System;
using UnityEngine;
namespace Simulator
{
    public static class SimulationAPI
    {
        static Simulation simulation;

        public static void Bind(Simulation sim)
        { simulation = sim; }

        public static void Request<T>(ISimulatorCommand command)
        {
            simulation.EnqueueCommand(command);
        }

        public static EventBus Events => simulation.Events;


        public static FactorySnapshot GetFactory(uint id)
        {
            return simulation.GetFactorySnapshot(id);
        }

        public static ConveyorSnapshot GetConveyor(uint id)
        {
            return simulation.GetConveyorSnapshot(id);
        }

        public static ManipulatorSnapshot GetManipulator(uint id)
        {
            return simulation.GetManipulator(id);
        }

        public static string GetScriptText(uint id)
        {
            return simulation.GetScriptText(id);
        }


        public static uint[] GetAllManipulator()
        {
            return simulation.GetAllManipulator();
        }

    }
}
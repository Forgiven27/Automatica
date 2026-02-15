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


        public static FactorySnapshot GetFactory(string id)
        {
            return simulation.GetFactorySnapshot(id);
        }

        public static ConveyorSnapshot GetConveyor(string id)
        {
            return simulation.GetConveyorSnapshot(id);
        }
    }
}
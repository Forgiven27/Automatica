using System.Collections.Generic;

namespace Simulator
{
    public struct ManipulatorCreatedEvent : ISimulationEvent
    {
        public uint ID;
        public float baseYaw;
        public TransformSim transform;
        public Dictionary<uint, float> bones;
    }
}
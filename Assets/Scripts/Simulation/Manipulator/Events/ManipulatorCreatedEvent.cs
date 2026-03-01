using System.Collections.Generic;
using UnityEngine;
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
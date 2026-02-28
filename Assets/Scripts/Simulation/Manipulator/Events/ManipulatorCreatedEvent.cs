using UnityEngine;
namespace Simulator
{
    public struct ManipulatorCreatedEvent : ISimulationEvent
    {
        public TransformSim transform;
        public Bone[] bones;
    }
}
using UnityEngine;
namespace Simulator { 
    public struct ConveyorCreatedEvent : ISimulationEvent
    {
        public uint conveyorID;
        public uint[] segmentsID;
        public TransformSim[] segmentsTransform;
    }
}
using UnityEngine;
namespace Simulator { 
    public struct ConveyorCreatedEvent : ISimulationEvent
    {
        public string conveyorID;
        public Vector3 startPosition;
        public Vector3 endPosition;
    }
}
using UnityEngine;

namespace Simulator
{
    public class FactoryCreatedEvent : ISimulationEvent
    {
        public uint factoryId;
        public FactoryType factoryType;
        public Vector3 position;
        public Quaternion rotation;
    }
}

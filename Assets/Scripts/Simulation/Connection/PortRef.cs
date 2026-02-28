using UnityEngine;

namespace Simulator
{ 
    public struct PortRef
    {
        public uint entityId;
        public uint portId;

        public PortRef (uint portId, uint entityId)
        {
            this.portId = portId;
            this.entityId = entityId;
        }
    }
}
using UnityEngine;

namespace Simulator
{ 
    public struct PortRef
    {
        public string entityId;
        public string portId;

        public PortRef (string portId, string entityId)
        {
            this.portId = portId;
            this.entityId = entityId;
        }
    }
}
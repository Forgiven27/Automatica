using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

namespace Simulator
{
    public class ConnectionSystem
    {
        private UnorderedPairDictionary<PortRef> referencePairs = new();
        public bool TryGetRef(PortRef first, out PortRef second)
        {
            return referencePairs.TryGetValue(first, out second);
        }

        public List<Connection> GetAllConnections(string entityID, List<string> portsID)
        {
            List<Connection> connections = new List<Connection>();
            foreach (string portID in portsID)
            {
                PortRef port = new PortRef() { entityId = entityID, portId = portID };
                if (TryGetRef(port, out PortRef anotherPort))
                {
                    connections.Add(new Connection() { first = port, second = anotherPort });
                }
            }
            return connections;
        }

        public void Connect(PortRef first, PortRef second)
        {
            referencePairs.Add(first, second);
        }
        public void Connect(Connection connection)
        {
            referencePairs.Add(connection.first, connection.second);
        }
        public void Disconnect(PortRef first)
        {
            referencePairs.Remove(first);
        }
        public void Disconnect(Connection connection)
        {
            referencePairs.Remove(connection.first);
        }
    }
}
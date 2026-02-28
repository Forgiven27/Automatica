using UnityEngine;

namespace Simulator
{
    public struct ConnectPortsCommand : ISimulatorCommand
    {
        public Connection connection { get; private set; }

        public ConnectPortsCommand(Connection connection)
        {
            this.connection = connection;
        }

        public ConnectPortsCommand(uint notCreatedPortID, PortRef existingEntityPortRef)
        {
            this.connection = new Connection()
            {
                first = new PortRef() { entityId = 0, portId = notCreatedPortID },
                second = existingEntityPortRef
            };
        }

        public void Execute(CommandContext context)
        {
            PortRef firstRef = connection.first;
            PortRef secondRef = connection.second;

            IEntity firstEntity;
            IEntity secondEntity;
            if (!context.entities.TryGetEntity<IEntity>(firstRef.entityId, out firstEntity)) return;
            if (!context.entities.TryGetEntity<IEntity>(secondRef.entityId, out secondEntity)) return;
            
            if (firstEntity is IConnectable firstEndPoint && secondEntity is IConnectable secondEndPoint)
            {
                firstEndPoint.Connect(firstRef, secondRef);
                secondEndPoint.Connect(secondRef, firstRef);
            }
        }


    }
}
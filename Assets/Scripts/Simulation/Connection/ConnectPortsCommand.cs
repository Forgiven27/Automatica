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

        public ConnectPortsCommand(string notCreatedPortID, PortRef existingEntityPortRef)
        {
            this.connection = new Connection()
            {
                first = new PortRef() { entityId = "", portId = notCreatedPortID },
                second = existingEntityPortRef
            };
        }

        public void Execute(CommandContext context)
        {
            PortRef firstRef = connection.first;
            PortRef secondRef = connection.second;

            var firstEntity = context.entities.Get<IEntity>(firstRef.entityId);
            var secondEntity = context.entities.Get<IEntity>(secondRef.entityId);

            if (firstEntity is IConnectable firstEndPoint && secondEntity is IConnectable secondEndPoint)
            {
                firstEndPoint.Connect(firstRef, secondRef);
                secondEndPoint.Connect(secondRef, firstRef);
            }
        }


    }
}
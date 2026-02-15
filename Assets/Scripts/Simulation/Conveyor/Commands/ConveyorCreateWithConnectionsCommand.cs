namespace Simulator
{
    public struct ConveyorCreateWithConnectionsCommand : ISimulatorCommand
    {
        public ConveyorCreateCommand ConveyorCreateCommand { get; set; }
        public ConnectPortsCommand[] ConnectPortsCommand { get; set; }

        public void Execute(CommandContext context)
        {
            IEntity entity = context.conveyorSim.Create(ConveyorCreateCommand, context.simulation);
            context.entities.Add(entity);
            string createdID = entity.ID;
            foreach (var cp in ConnectPortsCommand)
            {
                if (cp.connection.first.entityId == "")
                {
                    ConnectPortsCommand fullConnectCommand = new ConnectPortsCommand(new Connection()
                    {
                        first = new PortRef()
                        {
                            entityId = createdID,
                            portId = cp.connection.first.portId,
                        },
                        second = cp.connection.second

                    });
                    fullConnectCommand.Execute(context);
                }
            }
        }
    }
} 
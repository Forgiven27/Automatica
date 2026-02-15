using System.Collections.Generic;

namespace Simulator { 
    public class Simulation
    {
        public readonly EventBus Events = new();

        Queue<ISimulatorCommand> commands = new();

        FactorySimulation factorySim = new();
        ConveyorSimulation conveyorSim = new();
        EntityRegistry entities = new();
        ConnectionSystem connections = new();
        List<SimOperation> operations = new();

        public void EnqueueCommand(ISimulatorCommand command)
        {
            commands.Enqueue(command);
        }

        public void ProcessCommands()
        {
            var commandContext = new CommandContext()
            {
                simulation = this,
                connections = connections,
                conveyorSim = conveyorSim,
                factorySim = factorySim,
                entities = entities,
            };
            while (commands.TryDequeue(out var cmd))
            {
                cmd.Execute(commandContext);
            }
        }

        public void Tick()
        {
            ProcessCommands();

            operations.Clear();

            factorySim.Tick(this);
            ApplyInputOperations();
            ApplyOutputOperations();
            conveyorSim.Tick();
        }

        private void ApplyInputOperations()
        {
            foreach(var operation in operations)
            {
                if (operation.type == SimOpType.TakeFromConnection)
                {
                    var recieverRequest = entities.Get<IEntity>(operation.entityOpAnother.entityId);
                    var senderRequest = entities.Get<IEntity>(operation.entityOpAuthor.entityId);

                    if (recieverRequest is IItemSource src && senderRequest is IItemSink sink)
                    {
                        if (src.TryExport(operation.items, out List<ItemType> itemCanExport))
                        {
                            foreach (var itemType in itemCanExport) 
                            {
                                if (sink.TryImport(itemType))
                                {
                                    src.Export(itemType);
                                    sink.Import(itemType);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ApplyOutputOperations()
        {
            foreach (var operation in operations)
            {
                if (operation.type == SimOpType.PutToConnection)
                {
                    var recieverRequest = entities.Get<IEntity>(operation.entityOpAnother.entityId);
                    var senderRequest = entities.Get<IEntity>(operation.entityOpAuthor.entityId);

                    if (senderRequest is IItemSource src && recieverRequest is IItemSink sink)
                    {
                        if (src.TryExport(operation.items, out List<ItemType> itemCanExport))
                        {
                            foreach (var itemType in itemCanExport)
                            {
                                if (sink.TryImport(itemType))
                                {
                                    src.Export(itemType);
                                    sink.Import(itemType);
                                }
                            }
                        }
                    }
                }
            }
        }
        public FactoryContext CreateFactoryContext(string factoryId)
        {
            return new FactoryContext(factoryId, operations);
        }





        public FactorySnapshot GetFactorySnapshot(string id)
        {
            return factorySim.GetSnapshotById(id);
        }

        public ConveyorSnapshot GetConveyorSnapshot(string id)
        {
            return conveyorSim.GetSnapshotById(id);
        }

        
    }
}
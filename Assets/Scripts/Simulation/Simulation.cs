using System.Collections.Generic;

using System.Text;
using UnityEngine;
namespace Simulator { 
    public class Simulation
    {
        public readonly EventBus Events = new();

        Queue<ISimulatorCommand> _commands = new();

        FactorySimulation _factorySim = new();
        ConveyorSimulation _conveyorSim = new();
        EntityRegistry _entities = new();
        ConnectionSystem _connections = new();
        List<SimOperation> _operations = new();
        WorldQuerySimulation _worldQuery;
        CollisionSimulation _collisionSystem;
        public Simulation()
        {
            _collisionSystem = new CollisionSimulation(new CollisionConfig()
            {
                CellSize = 1,
            }, _entities);
            _worldQuery = new WorldQuerySimulation(_collisionSystem.StaticGrid, _collisionSystem.DynamicGrid);
        }

        public void EnqueueCommand(ISimulatorCommand command)
        {
            _commands.Enqueue(command);
        }

        public void ProcessCommands()
        {
            var commandContext = new CommandContext(this, _factorySim,
                _conveyorSim, _collisionSystem, _entities, _connections,
                _worldQuery);
            
            while (_commands.TryDequeue(out var cmd))
            {
                cmd.Execute(commandContext);
            }
        }

        public void Tick()
        {
            ProcessCommands();

            _operations.Clear();

            _factorySim.Tick(this);
            ApplyInputOperations();
            ApplyOutputOperations();
            _conveyorSim.Tick();
        }

        private void ApplyInputOperations()
        {
            foreach(var operation in _operations)
            {
                if (operation.type == SimOpType.TakeFromConnection)
                {
                    IEntity recieverRequest;
                    IEntity senderRequest;

                    if (!_entities.TryGetEntity<IEntity>(operation.entityOpAnother.entityId, out recieverRequest)) continue;
                    if (!_entities.TryGetEntity<IEntity>(operation.entityOpAuthor.entityId, out senderRequest)) continue;

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
            foreach (var operation in _operations)
            {
                if (operation.type == SimOpType.PutToConnection)
                {
                    IEntity recieverRequest;
                    IEntity senderRequest;

                    if (!_entities.TryGetEntity<IEntity>(operation.entityOpAnother.entityId, out recieverRequest)) continue;
                    if (!_entities.TryGetEntity<IEntity>(operation.entityOpAuthor.entityId, out senderRequest)) continue;


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

        public void MoveEntity(uint id, TransformSim newTransform)
        {
            if (!_entities.TryGetTransform(id, out var oldTransform))
                return;

            _entities.SetTransform(id, newTransform);

            _collisionSystem.UpdateTransform(id, newTransform);
        }
        public FactoryContext CreateFactoryContext(uint factoryId)
        {
            return new FactoryContext(factoryId, _operations);
        }

        public ManipulatorContext CreateManipulatorContext()
        {
            return new ManipulatorContext() { operations = _operations};
        }



        public FactorySnapshot GetFactorySnapshot(uint id)
        {
            return _factorySim.GetSnapshotById(id);
        }

        public ConveyorSnapshot GetConveyorSnapshot(uint id)
        {
            return _conveyorSim.GetSnapshotById(id);
        }

        
    }
}
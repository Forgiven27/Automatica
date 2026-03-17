using System.Collections.Generic;


namespace Simulator { 
    public class Simulation
    {
        public readonly EventBus Events = new();

        Queue<ISimulatorCommand> _commands = new();

        FactorySimulation _factorySim = new();
        ConveyorSimulation _conveyorSim = new();
        ManipulatorSimulation _manipulatorSim = new();
        EntityRegistry _entities = new();
        ConnectionSystem _connections = new();
        List<SimOperation> _operations = new();
        List<SimOperation> _moveOperations = new();
        WorldQuerySimulation _worldQuery;
        CollisionSimulation _collisionSystem;
        public Simulation(SimulationConfiguration config)
        {
            _collisionSystem = new CollisionSimulation(new CollisionConfig()
            {
                CellSize = config.cellSize,
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
                _conveyorSim, _manipulatorSim, _collisionSystem,
                _entities, _connections, _worldQuery);
            
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
            ApplyGrabOperations();
            _conveyorSim.Tick();
            _manipulatorSim.Tick(this);
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
        private void ApplyGrabOperations()
        {
            foreach (var operation in _operations)
            {
                if (operation.type != SimOpType.GrabItem)
                    continue;

                uint manipulatorId = operation.entityOpAuthor.entityId;
                uint itemOwnerId = operation.entityOpAnother.entityId;

                if (!_entities.TryGetEntity<ManipulatorRuntime>(manipulatorId, out var manip))
                    continue;

                if (!_entities.TryGetEntity<IEntity>(itemOwnerId, out var itemOwnerEntity))
                    continue;

                if (!(itemOwnerEntity is IItemSource source))
                    continue;

                if (!source.TryExport())
                    continue;

                if (manip.Logic.HeldItem != null)
                    continue;
                
                manip.Logic.HeldItem = source.Export();
            }

            foreach (var operation in _operations)
            {
                if (operation.type != SimOpType.ReleaseItem)
                    continue;

                uint manipulatorId = operation.entityOpAuthor.entityId;
                uint itemSinkId = operation.entityOpAnother.entityId;

                if (!_entities.TryGetEntity<ManipulatorRuntime>(manipulatorId, out var manip))
                    continue;

                if (!_entities.TryGetEntity<IEntity>(itemSinkId, out var itemSinkEntity))
                    continue;

                if (!(itemSinkEntity is IItemSink sink))
                    continue;

                if (manip.Logic.HeldItem == null)
                    continue;

                ItemType item = manip.Logic.HeldItem ?? ItemType.None;
                if (!sink.TryImport(manip.Logic.HeldItem ?? ItemType.None))
                    continue;

                manip.Logic.HeldItem = null;
                sink.Import(item);
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
            return new ManipulatorContext(_operations, _collisionSystem, _worldQuery);
        }



        public FactorySnapshot GetFactorySnapshot(uint id)
        {
            return _factorySim.GetSnapshotById(id);
        }

        public ConveyorSnapshot GetConveyorSnapshot(uint id)
        {
            return _conveyorSim.GetSnapshotById(id);
        }

        public ManipulatorSnapshot GetManipulator(uint id)
        {
            return _manipulatorSim.GetSnapshotById(id);
        }
        public uint[] GetAllManipulator()
        {
            return _manipulatorSim.GetAllID();
        }

        public string GetScriptText(uint id)
        {
            return _manipulatorSim.GetScriptTextByID(id);
        }

        public Dictionary<uint, CollisionObject> GetCollisionSnapshot()
        {
            return _collisionSystem.GetObjects();
        }

    }
}
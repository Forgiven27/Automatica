using UnityEngine;
namespace Simulator
{
    public struct FactoryCreateCommand : ISimulatorCommand
    {
        public FactoryType factoryType;
        public Vector3 position;
        public Quaternion rotation;
        public FactoryDescription factoryDescription;

        public void Execute(CommandContext context)
        {
            IEntity entity = context.factorySim.Create(this, context.simulation);
            context.entities.Add(entity);
        }

    }
}
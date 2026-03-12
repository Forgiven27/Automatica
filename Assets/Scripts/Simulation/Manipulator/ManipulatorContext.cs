using System.Collections.Generic;
using UnityEngine;
namespace Simulator
{
    public class ManipulatorContext
    {
        public List<SimOperation> operations;
        public CollisionSimulation collision;
        public WorldQuerySimulation worldQuery;

        public ManipulatorContext(List<SimOperation> operations, CollisionSimulation collision, WorldQuerySimulation worldQuery)
        {
            this.operations = operations;
            this.collision = collision;
            this.worldQuery = worldQuery;
        }
    }
}
using Simulator;
using UnityEngine;

public class GridStaticCreateCommand : ISimulatorCommand
{
    public CollisionObject collisionObject;
    public TransformSim transform;
    public void Execute(CommandContext context)
    {
        context.collisionSim.RegisterStaticObjectWoutID(collisionObject);
        context.entities.Add(collisionObject, transform);
    }
}

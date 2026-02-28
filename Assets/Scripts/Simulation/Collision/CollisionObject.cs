using Simulator;
using System.Collections.Generic;

public class CollisionObject : IEntity
{
    public uint ID { get; set; }
    public bool IsDynamic;
    public List<CollisionShape> Shapes = new();
    public CollisionObject(bool isDynamic)
    {
        IsDynamic = isDynamic;
    }
    public CollisionObject(uint ID, bool isDynamic, List<CollisionShape> shapes)
    {
        this.ID = ID;
        IsDynamic = isDynamic;
        Shapes = shapes;
    }
}

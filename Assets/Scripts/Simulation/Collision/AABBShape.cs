
using Simulator;
public class AABBShape : CollisionShape
{
    public AABBShape(uint ownerId, CollisionLayer layer, AABB localAABB, AABB worldAABB)
    {
        OwnerId = ownerId;
        Layer = layer;
        LocalAABB = localAABB;
        WorldAABB = worldAABB;
}
}

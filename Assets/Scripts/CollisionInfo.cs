using Simulator;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CollisionInfo : MonoBehaviour
{
    [SerializeField] private CollisionLayer layer;
    public CollisionObject GetCollisionObject()
    {
        var colliders = GetComponentsInChildren<Collider>();

        CollisionObject collisionObject = new CollisionObject(false);
        Vector3 rootPosition = transform.position;
        TransformSim transformSim = new TransformSim(transform.position, transform.rotation, transform.localScale);
        foreach (Collider collider in colliders)
        {
            if (collider.isTrigger) continue;

            Bounds worldBounds = collider.bounds;
            Bounds b = collider.bounds;

            Vector3 localMin = b.min - rootPosition;
            Vector3 localMax = b.max - rootPosition;

            AABB localAABB = new AABB(localMin, localMax);

            AABB worldAABB = CollisionShape.CalculateWorldAABB(localAABB, transformSim);
            AABBShape aABBShape = new(0, layer, localAABB, worldAABB);

            collisionObject.Shapes.Add(aABBShape);
        }
        return collisionObject;
    }
}

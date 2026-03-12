using Simulator;
using System.Linq;
using UnityEngine;

public class CollisionManipulatorInfo : MonoBehaviour
{
    [SerializeField] private CollisionLayer layer;
    [SerializeField] private ManipulatorDescription description;
    
    private CollisionObject collisionObject;
    private KinematicBone[] kinematicBones;
    private LogicBone[] logicBones;
    private ShapeBinding[] shapeBindings;
    
    

    private void Start()
    {
        logicBones = (LogicBone[])description.bones.Clone();
    }


    public (CollisionObject, KinematicBone[], LogicBone[], ShapeBinding[]) GetCreationInfo()
    {
        var colliders = GetComponentsInChildren<Collider>()
                .Where(c => !c.isTrigger)
                .ToArray();


        CollisionObject collisionObject = new CollisionObject(true);

        

        Vector3 rootPosition = transform.position;
        ObjectPart objectPart = null;
        TransformSim transformSim = new TransformSim(transform.position, transform.rotation, transform.localScale);
        kinematicBones = new KinematicBone[logicBones.Length];
        shapeBindings = new ShapeBinding[colliders.Length];
        int jointIndex = 0;
        int shapeBindingIndex = 0;
        foreach (Collider collider in colliders)
        {

            objectPart = collider.GetComponent<ObjectPart>();

            Bounds worldBounds = collider.bounds;
            Bounds b = collider.bounds;
            Vector3 center = collider.bounds.center;
            Debug.Log($"Center {center}");
            Vector3[] corners =
            {
                new Vector3(b.min.x, b.min.y, b.min.z),
                new Vector3(b.max.x, b.min.y, b.min.z),
                new Vector3(b.min.x, b.max.y, b.min.z),
                new Vector3(b.max.x, b.max.y, b.min.z),
                new Vector3(b.min.x, b.min.y, b.max.z),
                new Vector3(b.max.x, b.min.y, b.max.z),
                new Vector3(b.min.x, b.max.y, b.max.z),
                new Vector3(b.max.x, b.max.y, b.max.z)
            };

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var c in corners)
            {
                Vector3 local = transform.InverseTransformPoint(c);

                min = Vector3.Min(min, local);
                max = Vector3.Max(max, local);
            }

            AABB localAABB = new AABB(min, max);
            AABB worldAABB = new AABB(b.min, b.max);

            Debug.Log(worldAABB.Max - worldAABB.Min);
            
            
            if (objectPart.GetObjectType() == ObjectType.Joint)
            {
                kinematicBones[jointIndex] = new KinematicBone(logicBones[jointIndex].ID, 
                    objectPart.GetJointIndex(), collider.transform.localPosition, Vector3.right);
                jointIndex++;
            }
            int relatedJointIndex = objectPart.GetJointIndex();
            AABBShape aABBShape = new(0, layer, localAABB, worldAABB);

            Vector3 localPos = transform.InverseTransformPoint(collider.transform.position);
            Quaternion localRot = Quaternion.Inverse(transform.rotation) * collider.transform.rotation;

            TransformSim localTransform = new TransformSim(
                localPos,
                localRot,
                collider.transform.lossyScale
            );


            shapeBindings[shapeBindingIndex] = new ShapeBinding(aABBShape, relatedJointIndex,localTransform);
            shapeBindingIndex++;

            collisionObject.Shapes.Add(aABBShape);
        }
        return (collisionObject, kinematicBones, logicBones, shapeBindings);
    }
}

public struct BoneColliders
{
    public uint ID;
    public Collider[] colliders;
    
}

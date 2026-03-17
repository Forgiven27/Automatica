using Simulator;
using System;
using System.Collections.Generic;
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

    
        var jointParts = colliders
            .Select(c => c.GetComponent<ObjectPart>())
            .Where(op => op != null && op.GetObjectType() == ObjectType.Joint)
            .OrderBy(op => (int)op.GetJointID())
            .ToList();

        kinematicBones = new KinematicBone[jointParts.Count];
        shapeBindings = new ShapeBinding[colliders.Length];

        var jointIdToBoneIndex = new Dictionary<uint, int>();

        var jointIdToTransform = new Dictionary<uint, Transform>();

        int jointIndex = 0;
        int shapeBindingIndex = 0;

        foreach (Collider collider in colliders)
        {
            var objectPart = collider.GetComponent<ObjectPart>();
            if (objectPart == null) continue;

            uint jointId = objectPart.GetJointID();      
            int relatedJointIndex = objectPart.GetRelatedJointIndex();  
            ObjectType type = objectPart.GetObjectType();

            Transform parentTransform;

            if (relatedJointIndex < 0)
            {
                parentTransform = this.transform;
            }
            else
            {
                uint parentJointId = (uint)relatedJointIndex;
                parentTransform = jointIdToTransform.GetValueOrDefault(parentJointId);

                if (parentTransform == null)
                {
                    parentTransform = FindJointTransformById(parentJointId);
                    jointIdToTransform[parentJointId] = parentTransform;
                }
            }

            AABB localAABB = CalculateLocalAABB(collider, parentTransform);

            Vector3 localPos = parentTransform.InverseTransformPoint(collider.transform.position);
            Quaternion localRot = Quaternion.Inverse(parentTransform.rotation) * collider.transform.rotation;
            TransformSim localTransform = new TransformSim(localPos, localRot, Vector3.one);

            int thisBoneIndex = -1;  

            if (type == ObjectType.Joint)
            {
                int parentBoneIndex = -1;
                if (relatedJointIndex >= 0)
                {
                    parentBoneIndex = jointIdToBoneIndex.GetValueOrDefault((uint)relatedJointIndex, -1);
                }

                Vector3 localOffset = parentTransform.InverseTransformPoint(collider.transform.position);

                Vector3 axis = GetJointAxisFromDescription(jointId);

                kinematicBones[jointIndex] = new KinematicBone(
                    jointId,          
                    parentBoneIndex,   
                    localOffset,
                    axis
                );

                jointIdToBoneIndex[jointId] = jointIndex;
                jointIdToTransform[jointId] = collider.transform;
                thisBoneIndex = jointIndex;
                jointIndex++;
            }
            int shapeRelatedBoneIndex;

            if (relatedJointIndex < 0)
            {
                shapeRelatedBoneIndex = -1;
            }
            else
            {
                shapeRelatedBoneIndex = jointIdToBoneIndex.GetValueOrDefault((uint)relatedJointIndex, -1);
            }

            AABBShape aABBShape = new(0, layer, localAABB, new AABB());

            shapeBindings[shapeBindingIndex] = new ShapeBinding(
                aABBShape,
                shapeRelatedBoneIndex, 
                localTransform
            );
            shapeBindingIndex++;

            collisionObject.Shapes.Add(aABBShape);
        }

        if (shapeBindingIndex < shapeBindings.Length)
        {
            Array.Resize(ref shapeBindings, shapeBindingIndex);
        }

        return (collisionObject, kinematicBones, logicBones, shapeBindings);
    }


    private Transform FindJointTransformById(uint jointId)
    {
        var allParts = GetComponentsInChildren<ObjectPart>();

        foreach (var part in allParts)
        {
            if (part.GetObjectType() == ObjectType.Joint && part.GetJointID() == jointId)
                return part.transform;
        }

        Debug.LogError($"Joint с ID {jointId} не найден!");
        return this.transform; // Fallback на root
    }


    private AABB CalculateLocalAABB(Collider collider, Transform relativeTo)
    {
        Bounds worldBounds = collider.bounds;

        Vector3[] corners = new Vector3[8];
        corners[0] = new Vector3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z);
        corners[1] = new Vector3(worldBounds.max.x, worldBounds.min.y, worldBounds.min.z);
        corners[2] = new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.min.z);
        corners[3] = new Vector3(worldBounds.max.x, worldBounds.max.y, worldBounds.min.z);
        corners[4] = new Vector3(worldBounds.min.x, worldBounds.min.y, worldBounds.max.z);
        corners[5] = new Vector3(worldBounds.max.x, worldBounds.min.y, worldBounds.max.z);
        corners[6] = new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.max.z);
        corners[7] = new Vector3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z);

        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (var c in corners)
        {
            Vector3 local = relativeTo.InverseTransformPoint(c);
            min = Vector3.Min(min, local);
            max = Vector3.Max(max, local);
        }

        return new AABB(min, max);
    }

    private Vector3 GetJointAxisFromDescription(uint jointId)
    {

        for (int i = 0; i < logicBones.Length; i++)
        {
            if (logicBones[i].ID == jointId)
            {

                // return logicBones[i].Axis;
                return Vector3.right;
            }
        }
        return Vector3.right;
    }
}

public struct BoneColliders
{
    public uint ID;
    public Collider[] colliders;
    
}

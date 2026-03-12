using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulator
{
    public class ManipulatorKinematics
    {
        private readonly KinematicBone[] _bones;
        private readonly TransformSim[] _worldTransforms;

        public IReadOnlyList<TransformSim> WorldTransforms => _worldTransforms;

        public ManipulatorKinematics(KinematicBone[] bones)
        {
            _bones = bones;
            _worldTransforms = new TransformSim[bones.Length];
        }

        public void Update(Vector3 basePosition, float baseYaw, Dictionary<uint, float> angles)
        {
            TransformSim baseTransform = new TransformSim
            {
                position = basePosition,
                rotation = Quaternion.AngleAxis(baseYaw, Vector3.up)
            };

            for (int i = 0; i < _bones.Length; i++)
            {
                KinematicBone bone = _bones[i];

                float angle = 0f;
                if (angles.TryGetValue(bone.ID, out float a))
                    angle = a;

                Quaternion jointRotation = Quaternion.AngleAxis(angle, bone.Axis);

                TransformSim local = new TransformSim
                {
                    position = bone.LocalOffset,
                    rotation = jointRotation
                };

                if (bone.ParentIndex < 0)
                    _worldTransforms[i] = TransformSim.Combine(baseTransform, local);
                else
                    _worldTransforms[i] = TransformSim.Combine(_worldTransforms[bone.ParentIndex], local);
            }
        }
    }
    [Serializable]
    public struct KinematicBone
    {
        public uint ID;
        public int ParentIndex;
        public Vector3 LocalOffset;
        public Vector3 Axis;

        public KinematicBone(uint id, int parentIndex, Vector3 localOffset, Vector3 axis)
        {
            ID = id;
            ParentIndex = parentIndex;
            LocalOffset = localOffset;
            Axis = axis;
        }
    }
}
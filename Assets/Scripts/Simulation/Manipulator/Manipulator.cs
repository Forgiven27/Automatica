using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;

namespace Simulator
{
    public class Manipulator : IEntity
    {
        public uint ID { get; set; }

        private List<Bone> bones = new List<Bone>();

        private ProductionTimer timer;

        public bool IsExecuting { get; private set; }
        public enum ManipulatorState
        {
            Idle,
            ExecutingScript,
            Collision,
            JointLimitReached,
            NoItemToGrab,
            TargetUnreachable
        }


        // Базовая трансформация
        public Vector3 BasePosition;
        public float BaseYaw; // поворот основания вокруг Y

        // Суставы
        public JointState[] Joints;

        // Геометрия
        public SegmentDefinition[] Segments;

        // Результат FK
        public Vector3 EndEffectorPosition;
        public Quaternion EndEffectorRotation;

        // Захват
        public ItemType? HeldItem;

        // Состояние
        public ManipulatorState State;

        // Очередь действий из скрипта
        public ManipulatorCommandBuffer CommandBuffer;
        private ManipulatorContext context;
        public Manipulator()
        {
            timer = new ProductionTimer();

        }


        public void Work(ManipulatorContext context)
        {
            if (timer.IsReady())
            {
                timer.Start(10);
                this.context = context;
                DoScript();
            }
            else
            {
                timer.TryTick();
            }
        }

        private void DoScript()
        {
            switch (State)
            {
                case ManipulatorState.Idle:
                    break;
                case ManipulatorState.ExecutingScript:
                    ExecuteScript(); 
                    break;
                case ManipulatorState.Collision:
                    break;
                case ManipulatorState.JointLimitReached:
                    break;
                case ManipulatorState.NoItemToGrab:
                    break;
                case ManipulatorState.TargetUnreachable:
                    break;
            }
        }

        void ExecuteScript()
        {
            //ExecuteOperation();
            if (CommandBuffer.TargetBaseYaw != null && BaseYaw != CommandBuffer.TargetBaseYaw)
            {
                float angleStep = 10;
                BaseYaw = Mathf.MoveTowards(BaseYaw, CommandBuffer.TargetBaseYaw ?? BaseYaw, angleStep);
            }
            else if (CommandBuffer.TargetJointAngles != null && CommandBuffer.TargetJointAngles.Count != 0)
            {
                List<int> keysToRemove = new List<int>();
                foreach (var kvp in CommandBuffer.TargetJointAngles)
                {
                    Bone bone = bones.Find(x => x.ID == kvp.Key);
                    if (bone != null)
                    {
                        float newAngle = Mathf.MoveTowards(bone.joint.CurrentAngle, kvp.Value, bone.joint.MaxSpeedPerTick);
                        bone.joint.CurrentAngle = Mathf.Clamp(newAngle, bone.joint.MinAngle, bone.joint.MaxAngle);
                        if (bone.joint.CurrentAngle == kvp.Value)
                            keysToRemove.Add(kvp.Key);
                    }
                }
                foreach (var key in keysToRemove)
                {
                    CommandBuffer.TargetJointAngles.Remove(key);
                }

            }
            else if (CommandBuffer.RequestGrab)
            {
                context.operations.Add(new SimOperation()
                {
                    entityOpAuthor = new PortRef() { entityId = ID},
                    type = SimOpType.GrabItem,
                });
            }
            else if (CommandBuffer.RequestRelease)
            {
                context.operations.Add(new SimOperation()
                {
                    entityOpAuthor = new PortRef() { entityId = ID },
                    type = SimOpType.ReleaseItem,
                });
            }

        }

        private void TryGrab()
        {
            
        }

        private void TryRelease()
        {

        }



    }

    public struct JointState
    {
        public float CurrentAngle;
        public float MinAngle;
        public float MaxAngle;
        public float MaxSpeedPerTick;
    }

    public struct SegmentDefinition
    {
        public float Length;
        public float Radius; // для простых collision checks
    }

    public class Bone
    {
        public int ID;
        public SegmentDefinition segment;
        public JointState joint; //считается ближайшим к основанию
    }

    public class ManipulatorCommandBuffer
    {
        public float? TargetBaseYaw;
        public Dictionary<int, float> TargetJointAngles;

        public bool RequestGrab;
        public bool RequestRelease;
    }
}
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using System;

namespace Simulator
{
    public class Manipulator : IEntity
    {
        public uint ID { get; set; }
        public string ScriptText { get; set; }

        private Bone[] _bones;
        public Dictionary<uint, float> GetBonesSnapshot()
        {
            Dictionary<uint, float> boneSnapshot = new Dictionary<uint, float>();
            foreach (Bone bone in _bones)
            {
                boneSnapshot.Add(bone.ID, bone.joint.CurrentAngle);
            }
            return boneSnapshot;
        }

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

        public float BaseYaw; // поворот основания вокруг Y

        public JointState[] Joints;
        public SegmentDefinition[] Segments;

        // Результат FK
        public Vector3 EndEffectorPosition;
        public Quaternion EndEffectorRotation;

        public ItemType? HeldItem;


        public ManipulatorState State;
        private Queue<ManipulatorCommandBuffer> _commandQueue;
        private ManipulatorCommandBuffer CommandBuffer;

        private ManipulatorContext context;
        public Manipulator(float baseYaw, Bone[] bones)
        {
            timer = new ProductionTimer();
            _commandQueue = new Queue<ManipulatorCommandBuffer>();
            _bones = bones;
            BaseYaw = baseYaw;
        }
        public void AddCommands(Queue<ManipulatorCommandBuffer> commandQueue)
        {
            for (int i = 0; i < commandQueue.Count; i++)
            {
                _commandQueue.Enqueue(commandQueue.Dequeue());
            }
        }

        public void SetCommands(Queue<ManipulatorCommandBuffer> commandQueue)
        {    
            _commandQueue = commandQueue;   
        }

        public void Work(ManipulatorContext context)
        {
            if (timer.IsReady())
            {
                timer.Start(10);
                this.context = context;
                if (_commandQueue.Count > 0)
                {
                    State = ManipulatorState.ExecutingScript;
                }
                else
                {
                    State = ManipulatorState.Idle;
                }

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
                    if (_commandQueue.Count > 0)
                    {
                        State = ManipulatorState.ExecutingScript;
                    }
                    break;

                case ManipulatorState.ExecutingScript:
                    if (CommandBuffer == null)
                    {
                        if (_commandQueue.TryPeek(out CommandBuffer)) 
                            ExecuteCommand();
                    }
                    else
                    {
                        ExecuteCommand();
                    }

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

        void ExecuteCommand()
        {
            //ExecuteOperation();
            if (CommandBuffer == null) return;

            if (CommandBuffer.sleep > 0)
            {
                CommandBuffer.sleep -= 1;
            }
            else if (CommandBuffer.TargetBaseYaw != null && BaseYaw != CommandBuffer.TargetBaseYaw)
            {
                float angleStep = 10;
                BaseYaw = Mathf.MoveTowards(BaseYaw, CommandBuffer.TargetBaseYaw ?? BaseYaw, angleStep);
            }
            else if (CommandBuffer.TargetJointAngles != null && CommandBuffer.TargetJointAngles.Count != 0)
            {
                List<int> keysToRemove = new List<int>();
                foreach (var kvp in CommandBuffer.TargetJointAngles)
                {
                    for (int i = 0; i < _bones.Length;i++)
                    {
                        if (_bones[i].ID == kvp.Key)
                        {
                           
                            float newAngle = Mathf.MoveTowards(_bones[i].joint.CurrentAngle, kvp.Value, _bones[i].joint.MaxSpeedPerTick);
                            _bones[i].joint.CurrentAngle = Mathf.Clamp(newAngle, _bones[i].joint.MinAngle, _bones[i].joint.MaxAngle);
                            if (_bones[i].joint.CurrentAngle == kvp.Value)
                                keysToRemove.Add(kvp.Key);
                            break;
                        }
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
            if (CommandBuffer.RequestGrab == false &&
                CommandBuffer.RequestRelease == false &&
                (CommandBuffer.TargetBaseYaw == null|| CommandBuffer.TargetBaseYaw == BaseYaw)  &&
                CommandBuffer.TargetJointAngles.Count == 0 &&
                CommandBuffer.sleep == 0)
            {
                _commandQueue.Dequeue();
                CommandBuffer = null;
            }

        }

        private void TryGrab()
        {
            
        }

        private void TryRelease()
        {

        }



    }
    [Serializable]
    public struct JointState
    {
        public float CurrentAngle;
        public float MinAngle;
        public float MaxAngle;
        public float MaxSpeedPerTick;
    }
    [Serializable]
    public struct SegmentDefinition
    {
        public float Length;
        public float Radius; // для простых collision checks
    }
    [Serializable]
    public struct Bone
    {
        public uint ID;
        public SegmentDefinition segment;
        public JointState joint; //считается ближайшим к основанию
    }

    public class ManipulatorCommandBuffer
    {
        public float? TargetBaseYaw;
        public Dictionary<int, float> TargetJointAngles = new();

        public bool RequestGrab = false;
        public bool RequestRelease = false;

        public int sleep;
    }
}
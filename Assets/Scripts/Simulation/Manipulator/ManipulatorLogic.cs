using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using System;

namespace Simulator
{
    public class ManipulatorLogic
    {
        private ManipulatorRuntime _runtime;
        public string ScriptText { get; set; }

        private LogicBone[] _bones;
        private Dictionary<uint, float> boneSnapshot;
        public Dictionary<uint, float> BoneSnapshot 
        { 
            get 
            {
                foreach (LogicBone bone in _bones)
                {
                    boneSnapshot[bone.ID] =  bone.joint.CurrentAngle;
                }
                return boneSnapshot;
            } 
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
        private ManipulatorCommandBuffer _CommandBuffer;

        private ManipulatorContext _context;
        public ManipulatorLogic(float baseYaw, LogicBone[] bones)
        {
            timer = new ProductionTimer();
            _commandQueue = new Queue<ManipulatorCommandBuffer>();
            _bones = bones;
            BaseYaw = baseYaw;

            boneSnapshot = new Dictionary<uint, float>();
            foreach (LogicBone bone in _bones)
            {
                boneSnapshot.Add(bone.ID, bone.joint.CurrentAngle);
            }
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

        public void Work(ManipulatorContext context, ManipulatorRuntime manipulatorRuntime)
        {
            if (timer.IsReady())
            {
                timer.Start(10);
                _runtime = manipulatorRuntime;
                _context = context;
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
                    if (_CommandBuffer == null)
                    {
                        if (_commandQueue.TryPeek(out _CommandBuffer)) 
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
            if (_CommandBuffer == null) return;

            if (_CommandBuffer.sleep > 0)
            {
                _CommandBuffer.sleep -= 1;
            }
            else if (_CommandBuffer.TargetBaseYaw != null && BaseYaw != _CommandBuffer.TargetBaseYaw)
            {
                float angleStep = 10;
                BaseYaw = Mathf.MoveTowards(BaseYaw, _CommandBuffer.TargetBaseYaw ?? BaseYaw, angleStep);
            }
            else if (_CommandBuffer.TargetJointAngles != null && _CommandBuffer.TargetJointAngles.Count != 0)
            {
                List<int> keysToRemove = new List<int>();
                foreach (var kvp in _CommandBuffer.TargetJointAngles)
                {
                    for (int i = 0; i < _bones.Length;i++)
                    {
                        if (_bones[i].ID == kvp.Key)
                        {
                           
                            float newAngle = Mathf.MoveTowards(_bones[i].joint.CurrentAngle, kvp.Value, _bones[i].joint.MaxSpeedPerTick);
                            _bones[i].joint.CurrentAngle = Mathf.Clamp(newAngle, _bones[i].joint.MinAngle, _bones[i].joint.MaxAngle);
                            if (_bones[i].joint.CurrentAngle == kvp.Value
                                || (_bones[i].joint.MinAngle == _bones[i].joint.CurrentAngle 
                                     && _bones[i].joint.MinAngle > kvp.Value)
                                || (_bones[i].joint.MaxAngle == _bones[i].joint.CurrentAngle
                                     && _bones[i].joint.MaxAngle < kvp.Value))
                                keysToRemove.Add(kvp.Key);
                            break;
                        }
                    }
                }
                foreach (var key in keysToRemove)
                {
                    _CommandBuffer.TargetJointAngles.Remove(key);
                }
            }
            else if (_CommandBuffer.RequestGrab)
            {
                _runtime.CreateGrabOperation(_context);
                _CommandBuffer.RequestGrab = false;
            }
            else if (_CommandBuffer.RequestRelease)
            {
                _runtime.CreateReleaseOperation(_context);
                _CommandBuffer.RequestRelease = false;
            }
            if (_CommandBuffer.RequestGrab == false &&
                _CommandBuffer.RequestRelease == false &&
                (_CommandBuffer.TargetBaseYaw == null|| _CommandBuffer.TargetBaseYaw == BaseYaw)  &&
                _CommandBuffer.TargetJointAngles.Count == 0 &&
                _CommandBuffer.sleep == 0)
            {
                _commandQueue.Dequeue();
                _CommandBuffer = null;
            }
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
    public struct LogicBone
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
using UnityEngine;

namespace Simulator
{
    public struct TransformSim
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformSim(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        public static TransformSim Combine(TransformSim parent, TransformSim local) 
        {
            Vector3 newPosition = parent.position + parent.rotation * local.position;
            Quaternion newRotation = parent.rotation * local.rotation;

            return new TransformSim(newPosition, newRotation, Vector3.one);
        }
    }
}
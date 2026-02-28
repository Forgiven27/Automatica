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
    }
}
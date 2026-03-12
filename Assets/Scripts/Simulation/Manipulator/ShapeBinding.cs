
namespace Simulator
{
    public struct ShapeBinding
    {
        public CollisionShape Shape;
        public int RelatedJointIndex;
        public TransformSim LocalTransform;

        public ShapeBinding(CollisionShape shape, int relatedJointIndex, TransformSim localTransform)
        {
            Shape = shape;
            RelatedJointIndex = relatedJointIndex;
            LocalTransform = localTransform;
        }
    }
}
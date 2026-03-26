namespace DullVersion
{
    public class JointRotationOperation : Operation
    {
        public int jointIndex; // не уверен насчет этого
        public float angle;

        public void Execute(ManipulatorController controller)
        {
            controller.RotateJoint(jointIndex, angle);
        }
    }
}
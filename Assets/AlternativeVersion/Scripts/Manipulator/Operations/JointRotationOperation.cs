using static UnityEngine.Rendering.DebugUI;

namespace DullVersion
{
    public struct JointRotationOperation : Operation
    {
        public int jointIndex; // не уверен насчет этого
        public float angle;

        public bool TryInit(string[] args)
        {
            if (args == null || args.Length != 2) return false;
            else
            {
                if (!int.TryParse(args[0], out jointIndex)) return false;
                else if (!float.TryParse(args[1], out angle)) return false;
                else return true;
            }
        }
        public void Execute(ManipulatorController controller)
        {
            controller.RotateJoint(jointIndex, angle);
        }
    }
}
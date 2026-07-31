namespace DullVersion
{
    public struct WristGrabOperation : Operation
    {
        public bool TryInit(string[] args)
        {
            if (args == null || args.Length == 0) return true;
            else return false;
        }
        public void Execute(ManipulatorController controller)
        {
            controller.Grab();
        }
    }
}
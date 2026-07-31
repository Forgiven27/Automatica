namespace DullVersion
{
    public struct WaitItemOperation : Operation
    {
        public bool TryInit(string[] args)
        {
            if (args == null || args.Length == 0) return true;
            else return false;
        }
        public void Execute(ManipulatorController controller)
        {
            controller.EnableScaner();
        }
    }
}
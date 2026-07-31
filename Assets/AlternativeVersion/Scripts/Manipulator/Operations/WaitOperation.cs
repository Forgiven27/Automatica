namespace DullVersion
{
    public struct WaitOperation : Operation
    {
        public float value;

        public bool TryInit(string[] args)
        {
            if (args == null || args.Length != 1) return false;
            else
            {
                if (float.TryParse(args[0], out value) && value >= 0) return true;
                else return false;
            }
           
        }

        public void Execute(ManipulatorController controller)
        {
            controller.AddWaitTime(value);
        }
    }
}
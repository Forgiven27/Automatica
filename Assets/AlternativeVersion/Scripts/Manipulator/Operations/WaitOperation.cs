namespace DullVersion
{
    public class WaitOperation : Operation
    {
        public float value;

        public void Execute(ManipulatorController controller)
        {
            controller.AddWaitTime(value);
        }
    }
}
namespace DullVersion
{
    public class WaitItemOperation : Operation
    {
        public void Execute(ManipulatorController controller)
        {
            controller.EnableScaner();
        }
    }
}
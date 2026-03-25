namespace DullVersion
{
    public class WristReleaseOperation : Operation
    {
        public void Execute(ManipulatorController controller)
        {
            controller.Release();
        }
    }
}
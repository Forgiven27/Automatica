namespace DullVersion
{
    public class WristGrabOperation : Operation
    {
        public void Execute(ManipulatorController controller)
        {
            controller.Grab();
        }
    }
}
namespace Simulator
{
    public interface ISimulatorCommand
    {
        public void Execute(CommandContext context);
    }
}
using UnityEngine;
namespace DullVersion
{
    public interface Operation
    {
        public bool TryInit(string[] args);
        public void Execute(ManipulatorController controller);
    }
}
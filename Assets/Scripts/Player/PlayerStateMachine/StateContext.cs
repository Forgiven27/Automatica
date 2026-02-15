using UnityEngine;
namespace StateMachine
{
    public class StateContext
    {
        public InputController inputController { get; private set; }
        public MovementController movementController { get; private set; }
        public GroundChecker groundChecker { get; private set; }
        public BuildBoard buildBoard { get; private set; }
        public UIPlayerController uIPlayerController { get; private set; }
        public GhostSpawner GhostSpawner { get; private set; }
        public Camera buildCamera { get; private set; }
        public InputSystem_Actions InputSystem {  get; private set; }
        public StateContext(InputController inputController, 
            MovementController movementController, GroundChecker groundChecker,
            BuildBoard buildBoard, UIPlayerController uIPlayerController,
            GhostSpawner ghostSpawner, InputSystem_Actions inputSystem,
            Camera buildCamera)
        {
            this.inputController = inputController;
            this.movementController = movementController;
            this.groundChecker = groundChecker;
            this.buildBoard = buildBoard;
            this.uIPlayerController = uIPlayerController;
            GhostSpawner = ghostSpawner;
            InputSystem = inputSystem;
            this.buildCamera = buildCamera;
        }
    }
}
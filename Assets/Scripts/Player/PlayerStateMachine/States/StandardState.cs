using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
namespace StateMachine
{
    public class StandardState : IPlayerState
    {
        private PlayerStateMachine _playerStateMachine;
        private InputSystem_Actions.PlayerActions _inputPlayerActions;
        private InputActionController _actionController;
        private StateContext _context;
        public void Enter(PlayerStateMachine playerStateMachine, StateContext context)
        {
            _playerStateMachine = playerStateMachine;
            _actionController = playerStateMachine.actionController;
            _context = context;
            _inputPlayerActions = context.InputSystem.Player;
            Cursor.lockState = CursorLockMode.Locked;
            context.groundChecker.OnGround += context.movementController.Grounded;
            
        }

        public void Update()
        {
            _context.movementController.Move(_actionController.GetVector(_inputPlayerActions.Move));
            _context.movementController.MouseTrackingProcess(Mouse.current.delta.ReadValue());
            if (_actionController.IsActionReady(_inputPlayerActions.Jump))
            {
                _context.movementController.Jump();
            }
            if (_actionController.IsActionReadyWoutNotify(_inputPlayerActions.FirstSlot) || 
                _actionController.IsActionReadyWoutNotify(_inputPlayerActions.SecondSlot) ||
                _actionController.IsActionReadyWoutNotify(_inputPlayerActions.ThirdSlot) ||
                _actionController.IsActionReadyWoutNotify(_inputPlayerActions.ForthSlot) ||
                _actionController.IsActionReadyWoutNotify(_inputPlayerActions.FifthSlot))
            {
                _playerStateMachine.SwitchState(PlayerStateMachine.States.Building);
            }
            if (_actionController.IsActionReady(_inputPlayerActions.ControlPanel))
            {
                _playerStateMachine.SwitchState(PlayerStateMachine.States.ControlPanel);
            }
        }

        public void Exit()
        {
            _context.groundChecker.OnGround -= _context.movementController.Grounded;
        }
    }
}
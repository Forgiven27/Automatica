using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

            Hint[] hints = new Hint[4]
            {
                new Hint("Используйте WASD для перемещения" ),
                new Hint(_inputPlayerActions.Jump, "Нажмите, чтобы прыгать" ),
                new Hint(_inputPlayerActions.ControlPanel, "Нажмите, чтобы панель управления манипуляторами" ),
                new Hint(_inputPlayerActions.Cancel, "Нажмите, чтобы ВЫЙТИ в главное меню" ),
            };
            _context.uIPlayerController.GetHintController.SetHints(hints);
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
            if (_actionController.IsActionReady(_inputPlayerActions.Cancel))
            {
                Exit();
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }

        public void Exit()
        {
            _context.groundChecker.OnGround -= _context.movementController.Grounded;
        }
    }
}
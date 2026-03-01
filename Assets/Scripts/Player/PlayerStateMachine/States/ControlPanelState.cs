using StateMachine;
using UnityEngine;

public class ControlPanelState : IPlayerState
{
    private PlayerStateMachine _playerStateMachine;
    private StateContext _context;
    private InputSystem_Actions.PlayerActions _inputPlayerActions;
    private InputActionController _actionController;
    public void Enter(PlayerStateMachine playerStateMachine, StateContext context)
    {
        _playerStateMachine = playerStateMachine;
        _actionController = playerStateMachine.actionController;
        _context = context;
        _inputPlayerActions = context.InputSystem.Player;
        context.groundChecker.OnGround += context.movementController.Grounded;
        Cursor.lockState = CursorLockMode.None;
        _context.uIPlayerController.GetControlMonitor.gameObject.SetActive(true);
    }

    public void Update()
    {
        if (_actionController.IsActionReady(_inputPlayerActions.ControlPanel))
        {
            _playerStateMachine.SwitchState(PlayerStateMachine.States.Standard);
        }
    }

    public void Exit()
    {
        
        _actionController.ResetActions();
        _context.uIPlayerController.GetControlMonitor.gameObject.SetActive(false);
    }
}

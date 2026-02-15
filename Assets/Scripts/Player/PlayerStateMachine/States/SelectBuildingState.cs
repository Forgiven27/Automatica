using Simulator;
using StateMachine;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class SelectBuildingState : IPlayerState
{
    enum NumberKey
    {
        None,
        First,
        Second,
        Third,
        Forth,
        Fifth
    }


    private PlayerStateMachine _playerStateMachine;
    private InputSystem_Actions.PlayerActions _inputPlayerActions;
    private InputActionController _actionController;

    private StateContext _context;
    private BuildBoard _buildBoard;
    private NumberKey _stateBuffer;
    private UISelectBuildPanel _buildPanel;
    private bool _isShowSelectPanel;
    public void Enter(PlayerStateMachine playerStateMachine, StateContext context)
    {
        _playerStateMachine = playerStateMachine;
        _actionController = playerStateMachine.actionController;
        _context = context;
        _inputPlayerActions = context.InputSystem.Player;
        //context.groundChecker.OnGround += context.movementController.Grounded;
        _stateBuffer = NumberKey.None;
        _buildBoard = context.buildBoard;
        _buildPanel = _context.uIPlayerController.GetSelectBuildPanel;
        _isShowSelectPanel = false;
        
        if (_playerStateMachine.actionBuffer == _inputPlayerActions.FirstSlot)
        {
            _stateBuffer = NumberKey.First;
            ShowChoosePanel();
        }else if (_playerStateMachine.actionBuffer == _inputPlayerActions.SecondSlot)
        {

        }else if (_playerStateMachine.actionBuffer == _inputPlayerActions.ThirdSlot)
        {

        }else if (_playerStateMachine.actionBuffer == _inputPlayerActions.ForthSlot)
        {
            
        }else if (_playerStateMachine.actionBuffer == _inputPlayerActions.FifthSlot)
        {

        }
        else
        {
            Debug.LogError("Буфер действий не соответствует состоянию");
        }
        
    }
    public void Update()
    {
        if (_actionController.IsActionReady(_inputPlayerActions.Cancel) && _isShowSelectPanel)
        {
            _buildPanel.Hide();
            _isShowSelectPanel = false;
            //_playerStateMachine.IsFirstS_Tap = false;

            _playerStateMachine.SwitchState(PlayerStateMachine.States.Standard);
        }
        if (_actionController.IsActionReadyWoutNotify(_inputPlayerActions.Cancel))
        {
            Debug.Log("Cancel is true");
        }
    }


    void ShowChoosePanel()
    {
        BuildButton buildButton = new BuildButton();
        switch (_stateBuffer)
        {
            case NumberKey.None:
                return;
            case NumberKey.First:
                buildButton = _buildBoard.firstCell;
                break;
            case NumberKey.Second:
                buildButton = _buildBoard.secondCell;
                break;
            case NumberKey.Third:
                buildButton = _buildBoard.thirdCell;
                break;
            case NumberKey.Forth:
                buildButton = _buildBoard.forthCell;
                break;
            case NumberKey.Fifth:
                buildButton = _buildBoard.fifthCell;
                break;
        }
        _buildPanel.SetBuildings(buildButton);
        _buildPanel.Show();
        _isShowSelectPanel = true;
    }



    public void Exit()
    {
        _actionController.ResetActions();
    }

}

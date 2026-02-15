using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace StateMachine
{
    public class PlayerStateMachine
    {
        public enum States
        {
            Standard,
            Building,
            SelectBuilding
        }

        private StateContext _context;
        private IPlayerState _currentState;

        private Dictionary<States, IPlayerState> states;

        public InputActionController actionController;
        public InputAction actionBuffer;
        private string _currentStateName;

        public void Start(StateContext context)
        {
            _context = context;

            actionController = new(_context.InputSystem);

            _context.inputController.OnMove += (
                vector2 => actionController.SetVectorValue(_context.InputSystem.Player.Move, vector2));
            _context.inputController.OnJump_Tap += (
                () => actionController.TrySetActionReady(_context.InputSystem.Player.Jump));
            _context.inputController.OnPlaceBuilding_Tap += (
                () => actionController.TrySetActionReady(_context.InputSystem.Player.PlaceBuilding));
            
            _context.inputController.OnRotateR_Tap += (
                () => actionController.TrySetActionReady(_context.InputSystem.Player.RightRotate));
            _context.inputController.OnRotateL_Tap += (
                () => actionController.TrySetActionReady(_context.InputSystem.Player.LeftRotate));

            _context.inputController.OnFirstS_Tap += (
                () => actionController.TrySetActionReady(_context.InputSystem.Player.FirstSlot));
            _context.inputController.OnSecondS_Tap += (
                () => actionController.TrySetActionReady(_context.InputSystem.Player.SecondSlot));
            _context.inputController.OnThirdS_Tap += (
                () => actionController.TrySetActionReady(_context.InputSystem.Player.ThirdSlot));
            _context.inputController.OnForthS_Tap += (
                () => actionController.TrySetActionReady(_context.InputSystem.Player.ForthSlot));
            _context.inputController.OnFifthS_Tap += (
                () => actionController.TrySetActionReady(_context.InputSystem.Player.FifthSlot));


            _context.inputController.OnCancel_Tap += (() => actionController.TrySetActionReady(_context.InputSystem.Player.Cancel));



            states = new Dictionary<States, IPlayerState>()
            {
                { States.Standard, new StandardState() },
                { States.Building, new BuildingState() },
                { States.SelectBuilding, new SelectBuildingState() }
            };
            _currentState = states[States.Standard];

            _currentState.Enter(this, context);
        }


        public void Update()
        {
            _currentState.Update();
            if (_currentStateName != _currentState.ToString())
            {
                _currentStateName = _currentState.ToString();
                Debug.Log(_currentStateName);
            }
        }

        public void Stop()
        {
            _currentState.Exit();
        }

        public void SwitchState(States state)
        {
            _currentState.Exit();
            _currentState = states[state];
            _currentState.Enter(this, _context);
        }
    }
}
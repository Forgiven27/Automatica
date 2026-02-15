using Cysharp.Threading.Tasks;
using Simulator;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

namespace StateMachine
{
    public class BuildingState : IPlayerState
    {
        enum BuildState
        {
            None,
            Single,
            Double,
            Multiply
        }
        private PlayerStateMachine _playerStateMachine;
        private StateContext _context;
        private BuildBoard _buildBoard;
        private BuildState _stateBuffer;

        private GameObject _ghostBuilding;

        private InputSystem_Actions.PlayerActions _inputPlayerActions;
        private InputActionController _actionController;

        private SplineContainer _splineContainer;
        private int numKnot = 0;
        private bool _isActiveCamera = false;

        PortElement _firstPortElement;
        PortElement _secondPortElement;

        private Dictionary<BuildState, bool> buttonsActiveStates = new()
        {
            {BuildState.Single, true},
            {BuildState.Double, true},
            {BuildState.Multiply, true},
        };

        


        public void Enter(PlayerStateMachine playerStateMachine, StateContext context)
        {
            _playerStateMachine = playerStateMachine;
            _actionController = playerStateMachine.actionController;
            _context = context;
            _inputPlayerActions = context.InputSystem.Player;
            context.groundChecker.OnGround += context.movementController.Grounded;
            _stateBuffer = BuildState.None;
            _context.buildCamera.gameObject.SetActive(false);
            _buildBoard = context.buildBoard;
        }

        public void Update()
        {
            if (_stateBuffer == BuildState.None && _isActiveCamera)
            {
                _context.buildCamera.gameObject.SetActive(false);
                _isActiveCamera = false;
            }else if (_stateBuffer != BuildState.None && !_isActiveCamera)
            {
                _context.buildCamera.gameObject.SetActive(true);
                _isActiveCamera = true;
            }

            switch (_stateBuffer)
            {
                case BuildState.None:
                    DoNoneState();
                    break;
                case BuildState.Single:
                    DoSingleState();
                    break;
                case BuildState.Double:
                    DoDoubleState();
                    break;
                case BuildState.Multiply:

                    break;
            }

        
        }


        void DoNoneState()
        {
            MoveStandard();
            _actionController.IsActionReady(_inputPlayerActions.PlaceBuilding);

            if (_actionController.IsActionReady(_inputPlayerActions.FirstSlot))
            {
                if (_stateBuffer == BuildState.None && buttonsActiveStates[BuildState.Single])
                {
                    _stateBuffer = BuildState.Single;

                    TimerStandard(BuildState.Single).Forget();
                    return;
                }

            }


            if (_actionController.IsActionReady(_inputPlayerActions.SecondSlot))
            {
                if (_stateBuffer == BuildState.None && buttonsActiveStates[BuildState.Double])
                {
                    _stateBuffer = BuildState.Double;
                    TimerStandard(BuildState.Double).Forget();
                }
            }
            if (_actionController.IsActionReady(_inputPlayerActions.ThirdSlot))
            {
                if (_stateBuffer == BuildState.None && buttonsActiveStates[BuildState.Double])
                {
                    _stateBuffer = BuildState.Multiply;
                    //BuildMultiply();
                    _stateBuffer = BuildState.None;
                }
            }

            if (_actionController.IsActionReady(_inputPlayerActions.ForthSlot) ||
                _actionController.IsActionReady(_inputPlayerActions.FifthSlot))
            {

            }
        }

        void DoSingleState()
        {
            BuildSingle();
            if (_actionController.IsActionReady(_inputPlayerActions.FirstSlot) && buttonsActiveStates[BuildState.Single])
            {
                if (_ghostBuilding != null)
                {
                    _context.GhostSpawner.DestroyGhost(_ghostBuilding);
                    _ghostBuilding = null;
                }
                _playerStateMachine.actionBuffer = _inputPlayerActions.FirstSlot;
                _playerStateMachine.SwitchState(PlayerStateMachine.States.SelectBuilding);
            }
        }

        void BuildSingle()
        {
            MoveStandard();

            if (_actionController.IsActionReady(_inputPlayerActions.Cancel))
            {
                if (_ghostBuilding != null)
                {
                    _context.GhostSpawner.DestroyGhost(_ghostBuilding);
                    _ghostBuilding = null;
                }
                _stateBuffer = BuildState.None;
                _playerStateMachine.SwitchState(PlayerStateMachine.States.Standard);
                return;
            }


            if (_ghostBuilding == null)
            {
                _ghostBuilding = _context.GhostSpawner.InstantiateGhost(_buildBoard.firstCell.currentBuilding.buildingObject);
            }

            if (InputTool.TryGetSurfacePoint(out Vector3 point))
            {
                /*
                Vector3 newPosition = new Vector3(hitInfo.point.x + (hitInfo.point.x % step < step / 2 ? -hitInfo.point.x % step : step - hitInfo.point.x % step),
                    hitInfo.point.y,
                    hitInfo.point.z + (hitInfo.point.z % step < step / 2 ? -hitInfo.point.z % step : step - hitInfo.point.z % step))
                    + worldGrid.null_position;
                */
                _ghostBuilding.transform.position = point;

                if (_actionController.IsActionReady(_inputPlayerActions.LeftRotate)) 
                { 
                    
                    _ghostBuilding.transform.rotation *= Quaternion.Euler(0, -90, 0);
                    Debug.Log("Поворот налево");
                }
                if (_actionController.IsActionReady(_inputPlayerActions.RightRotate))
                {

                    _ghostBuilding.transform.rotation *= Quaternion.Euler(0, 90, 0);
                    Debug.Log("Поворот вправо");
                }                

                if (_actionController.IsActionReady(_inputPlayerActions.PlaceBuilding))
                {
                    Building building = _buildBoard.firstCell.currentBuilding;
                    switch (building.buildingType)
                    {
                        case BuildingType.Factory:
                            var c = new FactoryCreateCommand()
                            {
                                position = point,
                                rotation = _ghostBuilding.transform.rotation,
                                factoryDescription = (FactoryDescription)building.fullDescription,
                                factoryType = FactoryType.ExportImport10
                            };
                            SimulationAPI.Request<FactoryCreateCommand>(c);
                            break;
                        default:

                            break;

                    }
                    
                    _stateBuffer = BuildState.None;
                    _context.GhostSpawner.DestroyGhost(_ghostBuilding);
                    _ghostBuilding = null;
                    _playerStateMachine.SwitchState(PlayerStateMachine.States.Standard);
                }
            }
        }

        void DoDoubleState()
        {
            BuildDouble();
            if (_actionController.IsActionReady(_inputPlayerActions.SecondSlot) && buttonsActiveStates[BuildState.Double])
            {
                if (_ghostBuilding != null)
                {
                    _context.GhostSpawner.DestroyGhost(_ghostBuilding);
                    _ghostBuilding = null;
                }
                _playerStateMachine.actionBuffer = _inputPlayerActions.SecondSlot;
                _playerStateMachine.SwitchState(PlayerStateMachine.States.SelectBuilding);
            }
        }
        void BuildDouble()
        {
            MoveStandard();

            if (_actionController.IsActionReady(_inputPlayerActions.Cancel))
            {
                if (_ghostBuilding != null)
                {
                    _context.GhostSpawner.DestroyGhost(_ghostBuilding);
                    _ghostBuilding = null;
                    _splineContainer = null;
                }
                numKnot = 0;

                _stateBuffer = BuildState.None;
                _playerStateMachine.SwitchState(PlayerStateMachine.States.Standard);
                return;
            }
            if (_ghostBuilding == null)
            {
                _ghostBuilding = _context.GhostSpawner.InstantiateGhost(_buildBoard.secondCell.currentBuilding.buildingObject);
                _splineContainer = _ghostBuilding.GetComponent<SplineContainer>();
                _splineContainer.Spline.Clear();
                _splineContainer.Spline.Add(new BezierKnot(new Unity.Mathematics.float3(0,0,0)));
            }

            if (numKnot == 0)
            {
                if (InputTool.TryGetPoint("Building", out RaycastHit hitInfo))
                {
                    if (hitInfo.collider != null && hitInfo.collider.TryGetComponent(out PortElement portElement) && portElement.IOType == IOType.Output)
                    {
                        var offset = new Vector3(0,
                            -hitInfo.collider.bounds.size.y / 2,
                            0);
                        _ghostBuilding.transform.position = hitInfo.transform.position + offset;

                        _firstPortElement = portElement;
                    }
                }
                else if (InputTool.TryGetSurfacePoint(out Vector3 point))
                {
                    _ghostBuilding.transform.position = point;
                    _firstPortElement = null;
                }
                if (_actionController.IsActionReady(_inputPlayerActions.PlaceBuilding))
                {
                    numKnot++;
                    _splineContainer.Spline.Add(new BezierKnot(new Unity.Mathematics.float3(0, 0, 0)));
                }
            }
            else if (numKnot == 1)
            {
                if (InputTool.TryGetPoint("Building", out RaycastHit hitInfo))
                {
                    if (hitInfo.collider != null && hitInfo.collider.TryGetComponent(out PortElement portElement) &&
                        portElement != _firstPortElement && portElement.IOType == IOType.Input)
                    {
                        var point = hitInfo.point - _ghostBuilding.transform.position;
                        var offset = new Vector3(0,
                            -hitInfo.collider.bounds.size.y / 2,
                            0);
                        point += offset;
                        try
                        {
                            _splineContainer.Spline[1] = new BezierKnot(new Unity.Mathematics.float3(point.x, point.y, point.z));
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            Debug.Log($"Выход за границы {e.ToString()}");
                        }


                        _secondPortElement = portElement;
                    }
                }
                else
                if (InputTool.TryGetSurfacePoint(out Vector3 point))
                {
                    point = point - _ghostBuilding.transform.position;
                    try
                    {
                        _splineContainer.Spline[1] = new BezierKnot(new Unity.Mathematics.float3(point.x, point.y, point.z));
                    }catch (ArgumentOutOfRangeException e)
                    {
                        Debug.Log($"Выход за границы {e.ToString()}");
                    }
                    _secondPortElement = null;
                }
                if (_actionController.IsActionReady(_inputPlayerActions.PlaceBuilding))
                {
                    Vector3 startPosition = _splineContainer.Spline[0].Position;
                    Vector3 endPosition = _splineContainer.Spline[1].Position;

                    if (startPosition == endPosition) 
                        return;


                    var c = new ConveyorCreateCommand()
                    {
                        startPosition = startPosition + _ghostBuilding.transform.position,
                        endPosition = endPosition,
                        stepsOfContainer = Mathf.RoundToInt((startPosition - endPosition).magnitude),
                    };

                    if (_firstPortElement == null && _secondPortElement == null)
                    {

                        SimulationAPI.Request<ConveyorCreateCommand>(c);
                    }
                    else if (_firstPortElement != null && _secondPortElement == null)
                    {
                        var cpStart = new ConnectPortsCommand("0",
                            new PortRef(_firstPortElement.ID.ToString(),
                            _firstPortElement.GetComponentInParent<IEntity>().ID)
                            );

                        var cc = new ConveyorCreateWithConnectionsCommand()
                        {
                            ConveyorCreateCommand = c,
                            ConnectPortsCommand = new ConnectPortsCommand[] { cpStart },
                        };
                        _firstPortElement = null;
                        SimulationAPI.Request<ConveyorCreateWithConnectionsCommand>(cc);
                    }
                    else if (_firstPortElement == null && _secondPortElement != null)
                    {
                        var cpEnd = new ConnectPortsCommand("1",
                            new PortRef(_firstPortElement.ID.ToString(),
                            _firstPortElement.GetComponentInParent<IEntity>().ID)
                            );

                        var cc = new ConveyorCreateWithConnectionsCommand()
                        {
                            ConveyorCreateCommand = c,
                            ConnectPortsCommand = new ConnectPortsCommand[] { cpEnd },
                        };
                        _secondPortElement = null;
                        SimulationAPI.Request<ConveyorCreateWithConnectionsCommand>(cc);
                    }
                    else
                    {
                        var cpStart = new ConnectPortsCommand("0",
                            new PortRef(_firstPortElement.ID.ToString(),
                            _firstPortElement.GetComponentInParent<IEntity>().ID)
                            );
                        var cpEnd = new ConnectPortsCommand("1",
                            new PortRef(_secondPortElement.ID.ToString(),
                            _secondPortElement.GetComponentInParent<IEntity>().ID)
                            );

                        var cc = new ConveyorCreateWithConnectionsCommand()
                        {
                            ConveyorCreateCommand = c,
                            ConnectPortsCommand = new ConnectPortsCommand[] { cpStart, cpEnd },
                        };
                        _firstPortElement = null;
                        _secondPortElement = null;
                        SimulationAPI.Request<ConveyorCreateWithConnectionsCommand>(cc);
                    }


                    numKnot = 0;
                    _splineContainer = null;
                    _context.GhostSpawner.DestroyGhost(_ghostBuilding);
                    _ghostBuilding = null;

                    _stateBuffer = BuildState.None;
                    _playerStateMachine.SwitchState(PlayerStateMachine.States.Standard);
                }
            }


        }


        
        void MoveStandard()
        {
            _context.movementController.Move(_actionController.GetVector(_inputPlayerActions.Move));
            if (_actionController.IsActionReady(_inputPlayerActions.Jump))
            {
                _context.movementController.Jump();
            }
        }



        public void Exit()
        {
            _context.buildCamera.gameObject.SetActive(false);
            _isActiveCamera = false;
            _actionController.ResetActions();
        }


        private async UniTask TimerStandard(BuildState buildState)
        {
            buttonsActiveStates[buildState] = false;
            await UniTask.WaitForSeconds(1);
            buttonsActiveStates[buildState] = true;
        }

    }
}
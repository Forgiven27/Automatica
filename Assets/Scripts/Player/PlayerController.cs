using StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputController), typeof(MovementController),typeof(GroundChecker))]
public class PlayerController : MonoBehaviour
{

    public BuildBoard buildBoard;
    InputSystem_Actions _inputActions;
    PlayerStateMachine m_PlayerStateMachine = new();
    UIPlayerController _uIPlayerController;
    [SerializeField] private GhostSpawner _ghostSpawner;
    [SerializeField] private Camera _buildCamera;
    void Awake()
    {

        _uIPlayerController = GetComponent<UIPlayerController>();


        _inputActions = new InputSystem_Actions();

        _uIPlayerController.InitUI(buildBoard, _inputActions);
        
    }

    

    private void OnEnable()
    {

        StateContext context = new StateContext(
            GetComponent<InputController>(),
            GetComponent<MovementController>(),
            GetComponent<GroundChecker>(),
            buildBoard,
            GetComponent<UIPlayerController>(),
            _ghostSpawner,
            _inputActions,
            _buildCamera
            );

        m_PlayerStateMachine.Start(context);
    }

    private void Update()
    {
        m_PlayerStateMachine.Update();
    }


    private void OnDisable()
    {
        m_PlayerStateMachine.Stop();
    }

}

using UnityEngine;

[RequireComponent(typeof(InputController), typeof(MovementController),typeof(GroundChecker))]
public class PlayerController : MonoBehaviour
{
    InputController m_InputController;
    MovementController m_MovementController;
    GroundChecker m_GroundChecker;
    void Awake()
    {
        m_InputController = GetComponent<InputController>();
        m_MovementController = GetComponent<MovementController>();
        m_GroundChecker = GetComponent<GroundChecker>();
    }
    private void OnEnable()
    {
        m_GroundChecker.OnGround += m_MovementController.Grounded;
        m_InputController.OnJump_Tap += m_MovementController.Jump;
        m_InputController.OnMove += m_MovementController.Move;
    }

    private void OnDisable()
    {
        m_GroundChecker.OnGround -= m_MovementController.Grounded;
        m_InputController.OnJump_Tap -= m_MovementController.Jump;
        m_InputController.OnMove -= m_MovementController.Move;
    }

}

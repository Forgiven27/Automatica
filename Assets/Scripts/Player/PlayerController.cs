using UnityEngine;

[RequireComponent(typeof(InputController), typeof(MovementController),typeof(GroundChecker))]
public class PlayerController : MonoBehaviour
{
    InputController inputController;
    MovementController movementController;
    GroundChecker groundChecker;
    void Start()
    {
        inputController = GetComponent<InputController>();
        movementController = GetComponent<MovementController>();
        groundChecker = GetComponent<GroundChecker>();
    }

    
    void Update()
    {
        movementController.DoMove(inputController.GetMovement());
        movementController.DoJump(inputController.IsJump(), groundChecker.IsGrounded());
    }
}

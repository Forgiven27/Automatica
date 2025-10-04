using UnityEngine;

[RequireComponent(typeof(InputController), typeof(MovementController))]
public class PlayerController : MonoBehaviour
{
    InputController inputController;
    MovementController movementController;
    void Start()
    {
        inputController = GetComponent<InputController>();
        movementController = GetComponent<MovementController>();
    }

    
    void Update()
    {
        movementController.DoMove(inputController.GetMovement());
        movementController.DoJump(inputController.IsJump());
    }
}

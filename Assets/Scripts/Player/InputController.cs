using UnityEngine;
using UnityEngine.InputSystem;
public class InputController : MonoBehaviour
{
    
    private Vector2 m_Movement;
    private bool m_IsJump;

    public Vector2 GetMovement() => m_Movement;
    public bool IsJump() => m_IsJump;

    

    public void Move(InputAction.CallbackContext callbackContext)
    {
        m_Movement = callbackContext.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed)
        {
            m_IsJump = true;
        }
        else if(callbackContext.canceled)
        {
            m_IsJump = false;
        }
    }

}

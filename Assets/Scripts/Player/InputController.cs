using UnityEngine;
using UnityEngine.InputSystem;
public class InputController : MonoBehaviour
{
    
    private Vector2 m_Movement;
    private bool m_IsJump;
    private bool m_IsFirstSClick;
    private bool m_IsSecondSClick;
    private bool m_IsThirdSClick;
    private bool m_IsForthSClick;
    private bool m_IsFifthSClick;

    public Vector2 GetMovement() => m_Movement;
    public bool IsJump() => m_IsJump;
    public bool IsFirstSClick() => m_IsFirstSClick;
    public bool IsSecondSClick() => m_IsSecondSClick;
    public bool IsThirdSClick() => m_IsThirdSClick;
    public bool IsForthSClick() => m_IsForthSClick;
    public bool IsFifthSClick() => m_IsFifthSClick;



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
    public void FirstSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed) m_IsFirstSClick = true;
        else if (callbackContext.canceled) m_IsFirstSClick = false;
    }
    public void SecondSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed) m_IsSecondSClick = true;
        else if (callbackContext.canceled) m_IsSecondSClick = false;
    }
    public void ThirdSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed) m_IsThirdSClick = true;
        else if (callbackContext.canceled) m_IsThirdSClick = false;
    }
    public void ForthSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed) m_IsForthSClick = true;
        else if (callbackContext.canceled) m_IsForthSClick = false;
    }
    public void FifthSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.performed) m_IsFifthSClick = true;
        else if (callbackContext.canceled) m_IsFifthSClick = false;
    }







}

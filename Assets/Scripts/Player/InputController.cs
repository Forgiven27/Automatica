using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;
public class InputController : MonoBehaviour
{
    
    private Vector2 m_Movement;
    private bool m_IsJump;
    private bool m_IsFirstSClick;
    private bool m_IsSecondSClick;
    private bool m_IsThirdSClick;
    private bool m_IsForthSClick;
    private bool m_IsFifthSClick;
    private bool m_IsRotateLeftClick;
    private bool m_IsRotateRightClick;
    private bool m_IsPlaceBuilding;
    
    public delegate void MoveDelegate(Vector2 position);
    public delegate void TapDelegate();
    public delegate void PressDelegate();
    
    public event MoveDelegate OnMove;
    public event TapDelegate OnJump_Tap;
    public event TapDelegate OnFirstS_Tap;
    public event TapDelegate OnSecondS_Tap;
    public event TapDelegate OnThirdS_Tap;
    public event TapDelegate OnForthS_Tap;
    public event TapDelegate OnFifthS_Tap;
    public event TapDelegate OnRotateR_Tap;
    public event TapDelegate OnRotateL_Tap;
    public event TapDelegate OnPlaceBuilding_Tap;
    public event TapDelegate OnSwitchTypeStart_Tap;
    public event TapDelegate OnSwitchTypeEnd_Tap;




    public void Move(InputAction.CallbackContext callbackContext)
    {   
        OnMove?.Invoke(callbackContext.ReadValue<Vector2>());
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnJump_Tap?.Invoke();
    }
    public void FirstSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnFirstS_Tap?.Invoke();
    }
    public void SecondSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnSecondS_Tap?.Invoke();
    }
    public void ThirdSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnThirdS_Tap?.Invoke();
    }
    public void ForthSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnForthS_Tap?.Invoke();
    }
    public void FifthSlotTap(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnFifthS_Tap?.Invoke();
    }
    public void RotateLeft(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnRotateL_Tap?.Invoke();
    }
    public void RotateRight(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnRotateR_Tap?.Invoke();
    }
     
    public void PlaceBuilding(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnPlaceBuilding_Tap?.Invoke();
    }

    public void SwitchTypeStartConveyor(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnSwitchTypeStart_Tap?.Invoke();
    }
    public void SwitchTypeEndConveyor(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.interaction is TapInteraction) OnSwitchTypeEnd_Tap?.Invoke();
    }
}

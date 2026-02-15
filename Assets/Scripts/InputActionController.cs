using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputActionController
{
    private Dictionary<InputAction, bool2> _actionsIsReady;
    private Dictionary<InputAction, Vector2> _actionsVector;

    public InputActionController(InputSystem_Actions inputActions)
    {
        _actionsIsReady = new Dictionary<InputAction, bool2>()
        {
            { inputActions.Player.FirstSlot, new bool2(false,true)},
            { inputActions.Player.SecondSlot, new bool2(false,true)},
            { inputActions.Player.ThirdSlot, new bool2(false, true)},
            { inputActions.Player.ForthSlot, new bool2(false, true)},
            { inputActions.Player.FifthSlot, new bool2(false,true)},
            { inputActions.Player.Interact, new bool2(false, true)},
            { inputActions.Player.Jump, new bool2(false, true)},
            { inputActions.Player.PlaceBuilding, new bool2(false, true)},
            { inputActions.Player.LeftRotate, new bool2(false, true)},
            { inputActions.Player.RightRotate, new bool2(false, true)},
            { inputActions.Player.Cancel, new bool2(false, true)},
        };

        _actionsVector = new Dictionary<InputAction, Vector2>()
        {
            { inputActions.Player.Move, Vector2.zero },
            { inputActions.Player.Look, Vector2.zero },
        };
    }

    public bool IsActionReady(InputAction action)
    {
        if (_actionsIsReady[action].Equals(new bool2(true,true)))
        {
            TimerActionStandard(action).Forget();
            return true;
        }
        return false;
    }

    public bool IsActionReadyWoutNotify(InputAction action) => _actionsIsReady[action].Equals(new bool2(true,true));

    public void TrySetActionReady(InputAction action)
    {
        if (_actionsIsReady[action].y) 
        { 
            _actionsIsReady[action] = true; 
        }
    }

    public Vector2 GetVector(InputAction action)
    {
        return _actionsVector[action];
    }

    public void SetVectorValue(InputAction action, Vector2 vector)
    {
        _actionsVector[action] = vector;
    }

    private async UniTask TimerActionStandard(InputAction inputAction)
    {
        _actionsIsReady[inputAction] = new bool2(false, false);
        await UniTask.WaitForSeconds(0.1f);
        _actionsIsReady[inputAction] = new bool2(false, true);
    }

    public void ResetActions()
    {
        foreach (var kvp in _actionsIsReady.ToList())
        {
            _actionsIsReady[kvp.Key] = new bool2 (false, _actionsIsReady[kvp.Key].y);
        }
    }
}

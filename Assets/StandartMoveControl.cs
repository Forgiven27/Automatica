using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class StandartMoveControl : MonoBehaviour
{
    float speed_x = 5;

    float x_input;
    float y_input;
    float z_input;


    public void Move(InputAction.CallbackContext context)
    {
        Vector2 move = context.ReadValue<Vector2>();
        x_input = move.x;
        y_input = move.y;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        print((context.interaction).GetType());
    }

    public void Look(InputAction.CallbackContext context)
    {
        //print($"X = {context.ReadValue<Vector2>().x} | Y = {context.ReadValue<Vector2>().y}");
    }

    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            z_input = 1;
        }else
        if (Input.GetKey(KeyCode.Q))
        {
            z_input = -1;
        }
        else
        {
            z_input = 0;
        }
    }

    void FixedUpdate()
    {
        transform.Translate(new Vector3(x_input, z_input, y_input));
    }
}

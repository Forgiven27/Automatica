using System;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Camera m_Camera;
    void Start()
    {
        
    }

    void Update()
    {
        float x_move = Input.GetAxis("Horizontal");
        float y_move = Input.GetAxis("Vertical");
        float z_move = Convert.ToInt32(Input.GetKeyDown(KeyCode.E)) - Convert.ToInt32(Input.GetKeyDown(KeyCode.Q));
        
        

        transform.Translate(new Vector3(x_move, z_move, y_move));
    }

    private void FixedUpdate()
    {
        Vector3 direction = m_Camera.ScreenToWorldPoint(Input.mousePositionDelta);
        print(Input.mousePositionDelta);
        //transform.LookAt(direction);
        transform.Rotate(Input.mousePositionDelta);
    }
}

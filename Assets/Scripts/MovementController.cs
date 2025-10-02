using System;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Camera m_Camera;
    public float rotationSpeed = 1;
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
        Vector2 iRezolution = new Vector2(Screen.width, Screen.height);
        Vector2 uv = Input.mousePosition / iRezolution;
        Vector2 rotateSpeed = new Vector2();
        if (uv.x < 0.25)
        {
            rotateSpeed.x = -1 * rotationSpeed;
        }else if (uv.x > 0.75)
        {
            rotateSpeed.x = 1 * rotationSpeed;
        }
        else { 
        if (uv.y < 0.25)
        {
            rotateSpeed.y = 1;
        }
        else if (uv.y > 0.75)
        {
            rotateSpeed.y = -1;
        }
        }
        if (transform.rotation.eulerAngles.x != rotateSpeed.y * 30) {
            
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotateSpeed.y * 30, transform.rotation.eulerAngles.y, 0),0.2f);
        }
        transform.Rotate(new Vector3(0, rotateSpeed.x, 0));
    }
}

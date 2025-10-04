using System;
using Unity.VisualScripting;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Camera m_Camera;
    public float rotationSpeed = 1;
    public float movementSpeed;
    public float jumpForce;
    
    private Vector2 m_MoveVector;
    
    private bool m_IsJump;
    private float m_TimerJump = 0;
    public float cooldownJump = 5;
    private Rigidbody m_Rigidbody;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovementProcess();
        JumpProcess();
        MouseTrackingProcess();        
    }


    void MouseTrackingProcess()
    {
        Vector2 iRezolution = new Vector2(Screen.width, Screen.height);
        Vector2 uv = Input.mousePosition / iRezolution;
        Vector2 rotateSpeed = new Vector2();
        if (uv.x < 0.25)
        {
            rotateSpeed.x = -1 * rotationSpeed;
        }
        else if (uv.x > 0.75)
        {
            rotateSpeed.x = 1 * rotationSpeed;
        }
        else
        {
            if (uv.y < 0.25)
            {
                rotateSpeed.y = 1;
            }
            else if (uv.y > 0.75)
            {
                rotateSpeed.y = -1;
            }
        }
        if (transform.rotation.eulerAngles.x != rotateSpeed.y * 30)
        {

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotateSpeed.y * 30, transform.rotation.eulerAngles.y, 0), 0.2f);
        }
        //transform.Rotate(new Vector3(0, rotateSpeed.x, 0));
        //m_Rigidbody.AddTorque
    }

    void MovementProcess()
    {
        if (m_MoveVector == Vector2.zero) return;
        float x_move = m_MoveVector.x * movementSpeed * Time.deltaTime;
        float z_move = m_MoveVector.y * movementSpeed * Time.deltaTime;
        transform.Translate(new Vector3(x_move, 0, z_move));
    }

    void JumpProcess()
    {
        if (m_TimerJump > 0) m_TimerJump -= Time.deltaTime;
        if (m_IsJump && m_TimerJump <= 0)
        {
            print("JUMP");
            Vector3 jumpVector = Vector3.up * jumpForce;
            m_Rigidbody.AddForce(jumpVector, ForceMode.Impulse);
            m_TimerJump = cooldownJump;

        }
    }


    public void DoMove(Vector2 inputMove)
    {
        m_MoveVector = inputMove;
    }
    public void DoJump(bool inputJump)
    {
        m_IsJump = inputJump;
    }
}

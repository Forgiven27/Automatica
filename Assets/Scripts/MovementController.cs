using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class MovementController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera m_Camera;
    [SerializeField] private Vector2 rotateCoeff = new Vector3(1, 1);
    [SerializeField] private float minVert = -80f;
    [SerializeField] private float maxVert = 80f;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float cooldownJump = 5;


    enum Button
    {
        Jump,
    }

    private readonly float m_buttonJumpCooldown = 2f;
    private Dictionary<Button, bool> buttonsActiveState;


    bool m_IsGrounded;
    private bool m_IsJump;
    private Vector2 m_MoveVector;

    
    private Rigidbody m_Rigidbody;

    private float xRot;
    private float yRot;
    void Start()
    {
        buttonsActiveState = new Dictionary<Button, bool>() {
            {Button.Jump, true},
        };

        m_Rigidbody = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        Vector3 angles = transform.localEulerAngles;
        xRot = angles.x;
        yRot = angles.y;
        if (xRot > 180f) xRot -= 360f; // приводим к виду (-180;180)
    }

    void Update()
    {
        MouseTrackingProcess();

        Movement();

    }


    void MouseTrackingProcess()
    {

        Vector2 deltaMouseVector = Mouse.current.delta.ReadValue();

        // вычисляем новые углы вручную
        xRot -= deltaMouseVector.y * rotateCoeff.x;
        yRot += deltaMouseVector.x * rotateCoeff.y;

        // ограничиваем вертикальное вращение
        xRot = Mathf.Clamp(xRot, minVert, maxVert);

        // применяем к трансформу
        transform.rotation = Quaternion.Euler(xRot, yRot, 0f);
    }
    void Movement()
    {
        if (m_MoveVector == Vector2.zero) return;
        float x_move = m_MoveVector.x * movementSpeed * Time.deltaTime;
        float z_move = m_MoveVector.y * movementSpeed * Time.deltaTime;
        transform.Translate(new Vector3(x_move, 0, z_move));
    }


    public void Move(Vector2 moveVector)
    {
        m_MoveVector = moveVector;
    }

    public async void Jump()
    {

        if (buttonsActiveState[Button.Jump] && m_IsGrounded)
        {
            Vector3 jumpVector = Vector3.up * jumpForce;
            m_Rigidbody.AddForce(jumpVector, ForceMode.Impulse);
            buttonsActiveState[Button.Jump] = false;
            await TimerJump(Button.Jump);
        }
    }

    public void Grounded(bool isGrounded)
    {
        m_IsGrounded = isGrounded;
        
    }

    private async UniTask TimerJump(Button button)
    {
        await UniTask.WaitForSeconds(m_buttonJumpCooldown);
        buttonsActiveState[button] = true;
    }

}

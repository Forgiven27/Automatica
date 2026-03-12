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

    enum Button
    {
        Jump,
    }

    private readonly float m_buttonJumpCooldown = 2f;
    private Dictionary<Button, bool> buttonsActiveState;

    private bool m_IsGrounded;
    private Vector2 m_MoveVector;    
    private Rigidbody _rigidbody;

    private float xRot;
    private float yRot;
    void Start()
    {
        buttonsActiveState = new Dictionary<Button, bool>() {
            {Button.Jump, true},
        };

        _rigidbody = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        Vector3 angles = transform.localEulerAngles;
        xRot = angles.x;
        yRot = angles.y;
        if (xRot > 180f) xRot -= 360f; // приводим к виду (-180;180)
    }

    void Update()
    {
        Movement();
    }


    public void MouseTrackingProcess(Vector2 deltaMouseVector)
    {
        xRot -= deltaMouseVector.y * rotateCoeff.x;
        yRot += deltaMouseVector.x * rotateCoeff.y;

        xRot = Mathf.Clamp(xRot, minVert, maxVert);

        _rigidbody.rotation = Quaternion.Euler(xRot, yRot, 0f); ;
    }
    void Movement()
    {
        if (m_MoveVector == Vector2.zero) return;

        Vector3 move = _rigidbody.rotation * new Vector3(m_MoveVector.x, 0, m_MoveVector.y);
        move.y = 0;
        float x_move = m_MoveVector.x * movementSpeed * Time.deltaTime;
        float z_move = m_MoveVector.y * movementSpeed * Time.deltaTime;
        
        _rigidbody.MovePosition(_rigidbody.position + move * movementSpeed * Time.fixedDeltaTime);
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
            _rigidbody.AddForce(jumpVector, ForceMode.Impulse);
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

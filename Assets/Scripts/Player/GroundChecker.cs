using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField]
    private Vector3 relativePositionSphere;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float radius;

    bool isGrounded;
    
    public bool IsGrounded()=> isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position + relativePositionSphere, radius,groundMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + relativePositionSphere, radius);
    }
}

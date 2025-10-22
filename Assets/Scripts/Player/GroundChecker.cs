using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField]
    private Vector3 relativePositionSphere;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private float radius;
    bool prevIsGrounded;
    bool currIsGrounded;
    public delegate void GroundDelegate(bool isGrounded);
    public event GroundDelegate OnGround;

    private void Awake()
    {

        currIsGrounded = Physics.CheckSphere(transform.position + relativePositionSphere, radius, groundMask);
        prevIsGrounded = currIsGrounded;
        OnGround?.Invoke(currIsGrounded);
    }
    void Update()
    {
        currIsGrounded = Physics.CheckSphere(transform.position + relativePositionSphere, radius, groundMask);
        if (currIsGrounded != prevIsGrounded)
        {
            prevIsGrounded = currIsGrounded;
            OnGround?.Invoke(currIsGrounded);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = currIsGrounded? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position + relativePositionSphere, radius);
    }
}

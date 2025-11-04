using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UIElements;

public class RotateMove : MonoBehaviour
{
    public float angleInDegrees = 0;
    public float speed = 1;
    public LayerMask excludeMask;
    Vector3 direction = Vector3.forward;
    Transform parentTransform;
    Vector3 forward;

    void Start()
    {
        parentTransform = GetComponentInParent<Transform>();
        forward = parentTransform.transform.forward;
        direction = Quaternion.AngleAxis(angleInDegrees, Vector3.up) * forward;
    }
    private void FixedUpdate()
    {
        
        forward = parentTransform.transform.forward;
        direction = Quaternion.AngleAxis(angleInDegrees, Vector3.up) * forward;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer != excludeMask)
        collision.transform.position = collision.transform.position + speed * direction * Time.deltaTime;
    }
}

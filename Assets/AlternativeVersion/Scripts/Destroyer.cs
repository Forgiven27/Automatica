using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.TryGetComponent(out Rigidbody rb))
        {
            Destroy(rb.gameObject);
        }
    }
}

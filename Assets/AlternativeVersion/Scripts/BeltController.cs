using UnityEngine;
using System.Collections.Generic;

namespace DullVersion
{
    public class BeltController : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;
        List<Transform> transforms = new List<Transform>();
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Rigidbody rb))
            {
                if (rb.isKinematic) return;
            }
            if (!transforms.Contains(other.transform))
            {
                transforms.Add(other.transform);

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (transforms.Contains(other.transform))
            {
                transforms.Remove(other.transform);
            }
        }

        private void Update()
        {
            foreach (Transform t in transforms)
            {
                t.Translate(direction * Time.deltaTime, Space.World);
            }
        }
    }
}
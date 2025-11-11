using UnityEngine;

public class LineMove : MonoBehaviour
{

    Vector3 direction = Vector3.forward;
    void Start()
    {
        direction = GetComponentInParent<Transform>().transform.forward;
    }

    // Update is called once per frame
    

    private void OnCollisionStay(Collision collision)
    {
        collision.transform.position = collision.transform.position + direction * Time.deltaTime;
    }
}

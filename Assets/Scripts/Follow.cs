using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Follow : MonoBehaviour
{
    [SerializeField] GameObject followObject;
    void Start()
    {
        
    }

    
    void Update()
    {
        Vector3 dir = followObject.transform.position
            - transform.position;
        Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up);
        target = target * Quaternion.Euler(0, -90, 0);
        
        
        var forwardThis = Quaternion.LookRotation(transform.forward, Vector3.up);
        

        Vector3 e = target.eulerAngles;
        
        transform.rotation = Quaternion.Euler(e);
    }
}

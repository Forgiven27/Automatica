using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] Transform forearmBone;
    [SerializeField] Transform shoulderBone;
    [SerializeField] Transform target;
    Vector3 resultPos = Vector3.zero;
    float maxDist;
    void Start()
    {
        Vector3 virtualCenter = new Vector3(shoulderBone.position.x, shoulderBone.position.y, transform.position.z);
        maxDist = Vector3.Distance(transform.position, new Vector3(forearmBone.position.x, forearmBone.position.y, transform.position.z))
            + Vector3.Distance(new Vector3(forearmBone.position.x, forearmBone.position.y, transform.position.z), virtualCenter);
    }

    

    void Update()
    {
        if (Vector3.Distance(target.position, transform.position) >= 0.5)
        {
            Vector3 virtualCenter = new Vector3(shoulderBone.position.x, shoulderBone.position.y, transform.position.z);
            Vector3 vector = new Vector3(target.position.x, target.position.y, transform.position.z) - virtualCenter;
            Vector3 vectorN = vector.normalized;
            resultPos = vectorN * (Vector3.Distance(target.position, transform.position) >=maxDist? maxDist 
                : Vector3.Distance(new Vector3(target.position.x, target.position.y, transform.position.z), new Vector3(shoulderBone.position.x, shoulderBone.position.y, transform.position.z)));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(resultPos
                        + new Vector3(shoulderBone.position.x
                                        , shoulderBone.position.y
                                        , transform.position.z)
                        , 0.2f);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    public List<Transform> bones = new List<Transform>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (bones == null || bones.Count < 2) return;
        Gizmos.color = Color.blue;

        int n = 0;
        foreach (Transform t in bones)
        {
            Gizmos.DrawLine(bones[n].position, bones[n + 1].position);
            n++;
            
            if (n == bones.Count - 1) break;
        }
    }
}

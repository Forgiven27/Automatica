using Simulator;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDrawer : MonoBehaviour
{
#if UNITY_EDITOR
    Dictionary<uint, CollisionObject> objects;
    private void OnDrawGizmos()
    {
        try
        {
            objects = SimulationAPI.GetCollisionSnapshot();
        }
        catch
        {
            return;
        }
        

        foreach (var keyValuePair in objects)
        {
            switch (objects[keyValuePair.Key].Shapes[0].Layer)
            {
                case CollisionLayer.Manipulator:
                    Gizmos.color = Color.black;
                    break;
                case CollisionLayer.Conveyor:
                    Gizmos.color = Color.yellow;
                    break;
                case CollisionLayer.ItemInteractionZone:
                    Gizmos.color = Color.aquamarine;
                    break;
                default:
                    Gizmos.color = Color.green;
                    break;
            }

            foreach (var shape in objects[keyValuePair.Key].Shapes)
            {
                
                
                DrawAABB(shape.WorldAABB);
            }

        }
    }

    private void DrawAABB(AABB worldAABB)
    {
        Vector3 min = worldAABB.Min;
        Vector3 max = worldAABB.Max;
        
        //1-2
        Vector3 dot_1 = min;
        Vector3 dot_2 = new Vector3(max.x, min.y, min.z);
        Vector3 dot_3 = new Vector3(max.x, min.y, max.z);
        Vector3 dot_4 = new Vector3(min.x, min.y, max.z);

        Vector3 dot_11 = new Vector3(min.x, max.y, min.z);
        Vector3 dot_22 = new Vector3(max.x, max.y, min.z);
        Vector3 dot_33 = max;
        Vector3 dot_44 = new Vector3(min.x, max.y, max.z);


        Gizmos.DrawLine(dot_1, dot_2);
        Gizmos.DrawLine(dot_2, dot_3);
        Gizmos.DrawLine(dot_3, dot_4);
        Gizmos.DrawLine(dot_4, dot_1);

        Gizmos.DrawLine(dot_11, dot_22);
        Gizmos.DrawLine(dot_22, dot_33);
        Gizmos.DrawLine(dot_33, dot_44);
        Gizmos.DrawLine(dot_44, dot_11);

        Gizmos.DrawLine(dot_1, dot_11);
        Gizmos.DrawLine(dot_2, dot_22);
        Gizmos.DrawLine(dot_3, dot_33);
        Gizmos.DrawLine(dot_4, dot_44);
    }
#endif
}

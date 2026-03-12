using UnityEngine;
using Simulator;
public class SegmentView : MonoBehaviour, IEntity
{
    public uint ID {  get; set; }

    public void Bind(uint id)
    {
        ID = id;
    }
    [SerializeField] private LogicZoneInfo _zoneInfo;

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (_zoneInfo == null) return;
        Vector3 min = _zoneInfo.min;
        Vector3 max = _zoneInfo.max;
        Gizmos.color = Color.red;
        //1-2
        Vector3 dot_1 = min + transform.position;
        Vector3 dot_2 = new Vector3(max.x, min.y, min.z) + transform.position;
        Vector3 dot_3 = new Vector3(max.x, min.y, max.z) + transform.position;
        Vector3 dot_4 = new Vector3(min.x, min.y, max.z) + transform.position;

        Vector3 dot_11 = new Vector3(min.x, max.y, min.z) + transform.position;
        Vector3 dot_22 = new Vector3(max.x, max.y, min.z) + transform.position;
        Vector3 dot_33 = max + transform.position;
        Vector3 dot_44 = new Vector3(min.x, max.y, max.z) + transform.position;


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

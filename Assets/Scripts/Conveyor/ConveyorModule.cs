using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(SplineContainer))]
public class ConveyorModule : MonoBehaviour
{
    SplineContainer spline;
    [NonSerialized]public Transform startPoint;
    [NonSerialized]public Transform endPoint;
    ConveyorDescription m_ConveyorDescription;
    event Action OnConveyorDescChanged;
    List<GameObject> poolGO;
    public ConveyorDescription GetConveyorDescription()
    {
        if (m_ConveyorDescription == null) return null;
        return m_ConveyorDescription;
    }
    public void SetConveyorDescription(ConveyorDescription conveyorDescription)
    {
        m_ConveyorDescription = conveyorDescription;
        OnConveyorDescChanged.Invoke();
    }
    void OnEnable()
    {
        poolGO = new List<GameObject>();
        spline = GetComponent<SplineContainer>();
        Spline_changed();
        spline.Spline.changed += Spline_changed;
        OnConveyorDescChanged += Spline_changed;
    }

    private void Spline_changed()
    {
        if (m_ConveyorDescription == null) return;
        if(poolGO != null)  foreach (var go in poolGO) Destroy(go);
        var knots = spline.Spline.Knots;
        int countKnots = knots.Count<BezierKnot>();
        if (countKnots == 1)
        {
            var posFloat3_0 = knots.ElementAt<BezierKnot>(0).Position;
            var posLocal0 = new Vector3(posFloat3_0.x, posFloat3_0.y, posFloat3_0.z);
            var go = m_ConveyorDescription.prefab;
            poolGO.Add(Transform.Instantiate(go, posLocal0 + transform.position, Quaternion.identity, transform));
        }
        else
        if (countKnots == 2)
        {
            var posFloat3_0 = knots.ElementAt<BezierKnot>(0).Position;
            var posFloat3_1 = knots.ElementAt<BezierKnot>(1).Position;
            var posLocal0 = new Vector3(posFloat3_0.x, posFloat3_0.y, posFloat3_0.z);
            var posLocal1 = new Vector3(posFloat3_1.x, posFloat3_1.y, posFloat3_1.z);
            var go = m_ConveyorDescription.prefab;
            float size = 1;
            float distance = Vector3.Distance(posLocal0, posLocal1);
            float countGO = distance / size;
            float step = distance / countGO;
            Vector3 vector = posLocal1 - posLocal0;
            float xCorr = posLocal0.x - posLocal1.x;
            float zCorr = posLocal0.z - posLocal1.z;
            float sin = zCorr / distance;
            float cos = xCorr / distance;
            var rotation = Quaternion.LookRotation(posLocal1, Vector3.up);

            for (int i = 0; i < countGO - 1; i++)
            {
                poolGO.Add(Transform.Instantiate(go, posLocal0 + transform.position - new Vector3(-step / 2 * cos + step * i * cos, 0, step / 2 * sin + step * i * sin), rotation, transform));
            }
            if (countGO > 0)
            {
                startPoint = poolGO.First().transform;
                endPoint = poolGO.Last().transform;
            }
        }
    }
    

    
}

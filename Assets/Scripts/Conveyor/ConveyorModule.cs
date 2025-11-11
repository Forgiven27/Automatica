using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(SplineContainer))]
public class ConveyorModule : MonoBehaviour
{
    SplineContainer spline;
    Transform _startPoint;
    Transform _endPoint;
    public Transform startPoint{ get { return _startPoint; } private set {
        _startPoint = value;
        } }
    public Transform endPoint
    {
        get { return _endPoint; }
        private set
        {
            _endPoint = value;
        }
    }
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
    
    private void Awake()
    {
        poolGO = new List<GameObject>();
        spline = GetComponent<SplineContainer>();    
        spline.Spline.Clear();
        Spline_changed();
        spline.Spline.changed += Spline_changed;
        OnConveyorDescChanged += Spline_changed;
        
    }

    private void Spline_changed()
    {
        if (m_ConveyorDescription == null) return;
        if (poolGO != null) { foreach (var go in poolGO) Destroy(go);
            poolGO.Clear();
        }
        if (poolGO == null) poolGO = new List<GameObject>();
        var knots = spline.Spline.Knots;
        int countKnots = knots.Count<BezierKnot>();
        if (countKnots == 1)
        {
            var posFloat3_0 = knots.ElementAt<BezierKnot>(0).Position;
            var posLocal0 = new Vector3(posFloat3_0.x, posFloat3_0.y, posFloat3_0.z);
            var go = m_ConveyorDescription.prefab;
            var newConv = Transform.Instantiate(go, posLocal0 + transform.position, Quaternion.identity, transform);
            newConv.transform.localPosition = Vector3.zero;
            poolGO.Add(newConv);

            if (poolGO.Count > 0)
            {
                startPoint = poolGO[0].transform;
                endPoint = poolGO[poolGO.Count - 1].transform;
            }

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
                //var newConv = Transform.Instantiate(go, posLocal0 + transform.position - new Vector3(-step / 2 * cos + step * i * cos, 0, step / 2 * sin + step * i * sin), rotation, transform);
                var newConv = Transform.Instantiate(go, posLocal0 + transform.position, rotation, transform);
                newConv.transform.localPosition = Vector3.zero;
                newConv.transform.localPosition -= new Vector3(step * i * cos, 0, step * i * sin);
                poolGO.Add(newConv);
            }
            if (countGO > 0)
            {
                startPoint = poolGO[0].transform;
                endPoint = poolGO[poolGO.Count - 1].transform;
            }
        }
    }

    public void UpdateTransform()
    {
        if (poolGO != null && poolGO.Count > 0)
        {
            startPoint = poolGO[0].transform;
            endPoint = poolGO[poolGO.Count - 1].transform;
        }
    }
    private void OnDisable()
    {
        spline.Spline.changed -= Spline_changed;
        OnConveyorDescChanged -= Spline_changed;
    }



}

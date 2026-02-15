using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class ConveyorModule : MonoBehaviour
{
    SplineContainer spline;
    Transform _startPoint;
    Transform _endPoint;

    public List<ConveyorDescription> conveyorsStartDesc;
    public List<ConveyorDescription> conveyorsEndDesc;
    public ConveyorDescription baseStraightConveyor;
    public Action OnConveyorChanged;
    [Space]
    [SerializeField] private ConveyorElement _straightConveyorPrefab;

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
    
    List<ConveyorElement> _poolConveyorElements = new();
    
    
    

    private void Awake()
    {
        spline = GetComponent<SplineContainer>();    
        Spline_changed();
        spline.Spline.changed += Spline_changed;
    }

    public List<ConveyorElement> GetPoolConveyor() => _poolConveyorElements;

    private void Spline_changed()
    {
        if (_straightConveyorPrefab == null) return;
        if (_poolConveyorElements != null)
        {
            foreach (var convElem in _poolConveyorElements) Destroy(convElem.gameObject);
            _poolConveyorElements.Clear();
        }

        var knots = spline.Spline.Knots;

        int countKnots = knots.Count();

        if (countKnots == 1)
        {
            var posFloat3_0 = knots.ElementAt<BezierKnot>(0).Position;
            var posLocal0 = new Vector3(posFloat3_0.x, posFloat3_0.y, posFloat3_0.z);
            var go = _straightConveyorPrefab;

            var newConv = Instantiate(go, posLocal0 + transform.position, Quaternion.identity, transform);
        
            if (newConv.gameObject.layer != gameObject.layer) { 
                newConv.gameObject.layer = gameObject.layer;

                foreach(Transform child in newConv.transform)
                {
                    child.gameObject.layer = gameObject.layer;
                    foreach (Transform child2 in child.transform)
                    {
                        child2.gameObject.layer = gameObject.layer;
                    }
                }
            }
            newConv.IsFirstElement = true;
            newConv.transform.localPosition = Vector3.zero;
            _poolConveyorElements.Add(newConv);
            
            if (_poolConveyorElements.Count > 0)
            {
                startPoint = _poolConveyorElements[0].transform;
                endPoint = _poolConveyorElements[_poolConveyorElements.Count - 1].transform;
            }

        }
        else
        if (countKnots == 2)
        {
            var posFloat3_0 = knots.ElementAt<BezierKnot>(0).Position;
            var posFloat3_1 = knots.ElementAt<BezierKnot>(1).Position;
            var posLocal0 = new Vector3(posFloat3_0.x, posFloat3_0.y, posFloat3_0.z);
            var posLocal1 = new Vector3(posFloat3_1.x, posFloat3_1.y, posFloat3_1.z);
            var go = _straightConveyorPrefab;
            float size = 1;
            float distance = Vector3.Distance(posLocal0, posLocal1);
            float countGO = distance / size;
            if (countGO > 0) {
                float step = distance / countGO;
                Vector3 vector = posLocal1 - posLocal0;
                float xCorr = posLocal0.x - posLocal1.x;
                float zCorr = posLocal0.z - posLocal1.z;
                float sin = zCorr / distance;
                float cos = xCorr / distance;
                var rotation = Quaternion.LookRotation(posLocal1, Vector3.up);

                for (int i = 0; i < countGO; i++)
                {
                    var newConv = Instantiate(go, posLocal0 + transform.position, rotation, transform);
                    if (newConv.gameObject.layer != gameObject.layer)
                    {
                        newConv.gameObject.layer = gameObject.layer;

                        foreach (Transform child in newConv.transform)
                        {
                            child.gameObject.layer = gameObject.layer;
                            foreach (Transform child2 in child.transform)
                            {
                                child2.gameObject.layer = gameObject.layer;
                            }
                        }
                    }
                    if (i == countGO - 1)
                    {
                        newConv.IsLastElement = true;
                    }
                    if (i == 0)
                    {
                        newConv.IsFirstElement = true;
                    }

                    newConv.transform.localPosition = Vector3.zero;
                    newConv.transform.localPosition -= new Vector3(step * i * cos, 0, step * i * sin);
                    if (_poolConveyorElements.Count > 0)
                    {
                        _poolConveyorElements[_poolConveyorElements.Count - 1].NextConveyor = newConv;
                    }
                    _poolConveyorElements.Add(newConv);
                }
            }
            if (_poolConveyorElements.Count > 0)
            {
                startPoint = _poolConveyorElements[0].transform;
                endPoint = _poolConveyorElements[_poolConveyorElements.Count - 1].transform;
            }
        }

        OnConveyorChanged?.Invoke();
    }










    /*
    int currentEndDesc = 0;
    int currentStartDesc = 0;
    public void NextStartConveyorDesc()
    {
        currentStartDesc++;
        if (currentStartDesc >= conveyorsStartDesc.Count) currentStartDesc = 0;
        Spline_changed();
        
    }
    public void NextEndConveyorDesc()
    {
        currentEndDesc++;
        if (currentEndDesc >= conveyorsEndDesc.Count) currentEndDesc = 0;
        Spline_changed();
    }
    
    private void Spline_changed()
    {
        if (baseStraightConveyor == null || conveyorsEndDesc == null || conveyorsStartDesc == null) return;
        if (poolGO != null) 
        { 
            foreach (var go in poolGO) Destroy(go);
            poolGO.Clear();
        }else poolGO = new List<GameObject>();
        
        var knots = spline.Spline.Knots;
        
        int countKnots = knots.Count();
        
        if (countKnots == 1)
        {
            var posFloat3_0 = knots.ElementAt<BezierKnot>(0).Position;
            var posLocal0 = new Vector3(posFloat3_0.x, posFloat3_0.y, posFloat3_0.z);
            var go = conveyorsStartDesc[currentStartDesc].prefab;
            
            var newConv = Instantiate(go, posLocal0 + transform.position, Quaternion.identity, transform);
            newConv.transform.localPosition = Vector3.zero;
            poolGO.Add(newConv);
            print(newConv.name);
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
            var go = baseStraightConveyor.prefab;
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
                var newConv = new GameObject();
                //if (i == 0) newConv = Transform.Instantiate(conveyorsStartDesc[currentStartDesc].prefab, posLocal0 + transform.position, rotation, transform);
                //else if (i == countGO - 2) { newConv = Transform.Instantiate(conveyorsEndDesc[currentEndDesc].prefab, posLocal0 + transform.position, rotation, transform); }
                //else 
                
                newConv = Transform.Instantiate(go, posLocal0 + transform.position, rotation, transform);
                newConv.transform.localPosition = Vector3.zero;
                newConv.transform.localPosition -= new Vector3(step * i * cos, 0, step * i * sin);
                poolGO.Add(newConv);
            }
            if (poolGO.Count > 0)
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
    */
    private void OnDisable()
    {
        spline.Spline.changed -= Spline_changed;
    }
}

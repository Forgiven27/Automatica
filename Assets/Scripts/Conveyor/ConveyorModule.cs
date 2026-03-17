using Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    [SerializeField] private LogicZoneInfo _segmentLogicZone;
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
    
    public TransformSim[] GetSegmentsTransform()
    {
        TransformSim[] transforms = new TransformSim[_poolConveyorElements.Count];

        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i] = new TransformSim(_poolConveyorElements[i].transform.position,
                _poolConveyorElements[i].transform.rotation,
                _poolConveyorElements[i].transform.localScale);
        }

        return transforms;
    }

    public CollisionObject[] GetCollisionObjects()
    {
        CollisionObject[] collisions = new CollisionObject[_poolConveyorElements.Count];

        for (int i = 0; i < collisions.Length; i++)
        {
            GameObject segment = _poolConveyorElements[i].gameObject;
            Transform segmentTransform = segment.transform;
            Collider[] colliders = segment.GetComponentsInChildren<Collider>();

            TransformSim segmentWorldTransform = new TransformSim(
                segmentTransform.position,
                segmentTransform.rotation,
                segmentTransform.lossyScale);

            CollisionObject collisionObject = new CollisionObject(true);

            bool hasAnyCollider = false;
            Vector3 combinedLocalMin = Vector3.positiveInfinity;
            Vector3 combinedLocalMax = Vector3.negativeInfinity;

            foreach (Collider collider in colliders)
            {
                if (collider.isTrigger) continue; 

                Bounds worldBounds = collider.bounds;

                Vector3[] worldCorners = new Vector3[]
                {
                worldBounds.min,
                new Vector3(worldBounds.max.x, worldBounds.min.y, worldBounds.min.z),
                new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.min.z),
                new Vector3(worldBounds.max.x, worldBounds.max.y, worldBounds.min.z),
                new Vector3(worldBounds.min.x, worldBounds.min.y, worldBounds.max.z),
                new Vector3(worldBounds.max.x, worldBounds.min.y, worldBounds.max.z),
                new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.max.z),
                worldBounds.max
                };

                Vector3 localMin = Vector3.one * float.MaxValue;
                Vector3 localMax = Vector3.one * float.MinValue;
                foreach (Vector3 worldCorner in worldCorners)
                {
                    Vector3 localCorner = segmentTransform.InverseTransformPoint(worldCorner);
                    localMin = Vector3.Min(localMin, localCorner);
                    localMax = Vector3.Max(localMax, localCorner);
                }

                AABB localAABB = new AABB(localMin, localMax);

                if (!hasAnyCollider)
                {
                    combinedLocalMin = localMin;
                    combinedLocalMax = localMax;
                    hasAnyCollider = true;
                }
                else
                {
                    combinedLocalMin = Vector3.Min(combinedLocalMin, localMin);
                    combinedLocalMax = Vector3.Max(combinedLocalMax, localMax);
                }

                AABB worldAABB = CollisionShape.CalculateWorldAABB(localAABB, segmentWorldTransform);

                AABBShape shape = new AABBShape(0, CollisionLayer.Conveyor, localAABB, worldAABB);
                collisionObject.Shapes.Add(shape);
            }

            if (hasAnyCollider)
            {
                AABB combinedLocalAABB = new AABB(combinedLocalMin, combinedLocalMax);
                AABB combinedWorldAABB = CollisionShape.CalculateWorldAABB(combinedLocalAABB, segmentWorldTransform);
                AABBShape logicZoneShape = new AABBShape(0, CollisionLayer.ItemInteractionZone, combinedLocalAABB, combinedWorldAABB);
                collisionObject.Shapes.Add(logicZoneShape);
            }
            else
            {
                Debug.LogWarning($"Сегмент {segment.name} не содержит коллайдеров, логическая зона не создана.");
            }

            collisions[i] = collisionObject;
        }

        return collisions;
    }


    /*
    public CollisionObject[] GetCollisionObjects()
    {
        CollisionObject[] collisions = new CollisionObject[_poolConveyorElements.Count];

        for (int i = 0; i < collisions.Length; i++)
        {
            GameObject segment = _poolConveyorElements[i].gameObject;
            Transform root = segment.transform;

            var colliders = segment.GetComponentsInChildren<Collider>()
                .Where(c => !c.isTrigger)
                .ToArray();

            CollisionObject collisionObject = new CollisionObject(true);

            TransformSim transformSim =
                new TransformSim(root.position, root.rotation, root.localScale);

            if (colliders.Length == 0)
            {
                collisions[i] = collisionObject;
                continue;
            }

            // ===== 1. Суммарные bounds всех коллайдеров =====
            Bounds totalBounds = colliders[0].bounds;

            for (int c = 1; c < colliders.Length; c++)
                totalBounds.Encapsulate(colliders[c].bounds);

            // ===== 2. перевод bounds в local пространство сегмента =====
            Vector3[] corners =
            {
            new Vector3(totalBounds.min.x, totalBounds.min.y, totalBounds.min.z),
            new Vector3(totalBounds.max.x, totalBounds.min.y, totalBounds.min.z),
            new Vector3(totalBounds.min.x, totalBounds.max.y, totalBounds.min.z),
            new Vector3(totalBounds.max.x, totalBounds.max.y, totalBounds.min.z),
            new Vector3(totalBounds.min.x, totalBounds.min.y, totalBounds.max.z),
            new Vector3(totalBounds.max.x, totalBounds.min.y, totalBounds.max.z),
            new Vector3(totalBounds.min.x, totalBounds.max.y, totalBounds.max.z),
            new Vector3(totalBounds.max.x, totalBounds.max.y, totalBounds.max.z)
        };

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (var corner in corners)
            {
                Vector3 local = root.InverseTransformPoint(corner);
                min = Vector3.Min(min, local);
                max = Vector3.Max(max, local);
            }

            AABB localAABB = new AABB(min, max);
            AABB worldAABB = new AABB(totalBounds.min, totalBounds.max);

            AABBShape geometryShape =
                new AABBShape(0, CollisionLayer.Conveyor, localAABB, worldAABB);

            collisionObject.Shapes.Add(geometryShape);

            // ===== 3. logic zone =====

            Vector3 logicMin = root.InverseTransformPoint(_segmentLogicZone.min);
            Vector3 logicMax = root.InverseTransformPoint(_segmentLogicZone.max);

            AABB localLogicAABB = new AABB(logicMin, logicMax);

            AABB worldLogicAABB =
                CollisionShape.CalculateWorldAABB(localLogicAABB, transformSim);

            AABBShape logicShape =
                new AABBShape(0, CollisionLayer.ItemInteractionZone, localLogicAABB, worldLogicAABB);

            collisionObject.Shapes.Add(logicShape);

            collisions[i] = collisionObject;
        }

        return collisions;
    }

    
    public CollisionObject[] GetCollisionObjects()
    {
        CollisionObject[] collisions = new CollisionObject[_poolConveyorElements.Count];
        for (int i = 0; i < collisions.Length; i++)
        {
            GameObject segment = _poolConveyorElements[i].gameObject;
            var colliders = segment.GetComponentsInChildren<Collider>();

            CollisionObject collisionObject = new CollisionObject(true);
            Vector3 rootPosition = segment.transform.position;
            TransformSim transformSim = new TransformSim(transform.position, transform.rotation, transform.localScale);

            foreach (Collider collider in colliders)
            {
                if (collider.isTrigger) continue;

                Bounds worldBounds = collider.bounds;
                Bounds b = collider.bounds;

                Vector3 localMin = b.min - rootPosition;
                Vector3 localMax = b.max - rootPosition;

                AABB localAABB = new AABB(localMin, localMax);



                AABB worldAABB = new AABB(b.min, b.max);// CollisionShape.CalculateWorldAABB(localAABB, transformSim);
                
                AABBShape aABBShape = new(0, CollisionLayer.Conveyor, localAABB, worldAABB);

                collisionObject.Shapes.Add(aABBShape);
                
            }
            Vector3 localLogicMin = _segmentLogicZone.min - rootPosition;
            Vector3 localLogicMax = _segmentLogicZone.max - rootPosition;
            AABB localLogicAABB = new AABB(localLogicMin, localLogicMax);

            AABB worldLogicAABB = CollisionShape.CalculateWorldAABB(localLogicAABB, transformSim);
            AABBShape aABBLogicShape = new(0, CollisionLayer.ItemInteractionZone, localLogicAABB, worldLogicAABB);
            collisionObject.Shapes.Add(aABBLogicShape);

            collisions[i] = collisionObject;
        }
        return collisions;
    }
    */

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

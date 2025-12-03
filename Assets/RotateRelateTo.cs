using System;
using System.Collections.Generic;

using UnityEngine;

public class RotateRelateTo : MonoBehaviour
{
    [SerializeField]public List<RelatedObject> relativeTransforms = new List<RelatedObject>();
    [SerializeField] private Vector3 angleForCheck;
    [SerializeField] private Vector3 axisForCheck;
    Action OnTransformChanged;
    private Vector3 posOld;
    private Quaternion rotOld;
    private float deltaAngle;

    void Start()
    {
        posOld = transform.position;
        rotOld = transform.localRotation;
        OnTransformChanged += UpdateTransforms;
    }

    
    void Update()
    {
        StateCheck();
    }

    void StateCheck()
    {
        if (relativeTransforms.Count == 0) return;
        
        if (posOld != transform.position || Quaternion.Angle(rotOld, transform.localRotation) > 0.001f)
        {
            posOld = transform.position;
            deltaAngle = Vector3.SignedAngle(rotOld * angleForCheck,
                    transform.localRotation * angleForCheck,
                    axisForCheck);
            rotOld = transform.localRotation;
            
            OnTransformChanged?.Invoke();
        }
    }

    void UpdateTransforms()
    {
        foreach (var relatedObject in relativeTransforms)
        {
            //t.position += posOld;
            //t.localRotation = rotOld;
            Transform t = relatedObject.transform;
            Vector3 v = relatedObject.vector;
            t.RotateAround(transform.position, v, deltaAngle);
            print(deltaAngle);
        }
    }

    private void OnDestroy()
    {
        OnTransformChanged -= UpdateTransforms;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkRed;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
[Serializable]
public class RelatedObject
{
    public Transform transform;
    public Vector3 vector;
}

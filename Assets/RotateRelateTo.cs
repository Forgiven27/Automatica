using System;
using System.Collections.Generic;

using UnityEngine;

public class RotateRelateTo : MonoBehaviour
{
    [Serializable]
    public class RelatedObject
    {
        public Transform obj;
        public Vector3 initialLocalPos;
        public Quaternion initialLocalRot;
    }

    public List<RelatedObject> related = new();

    private Quaternion oldRot;
    private Vector3 oldPos;

    void Start()
    {
        oldRot = transform.rotation;
        oldPos = transform.position;

        foreach (var r in related)
        {
            // Запоминаем положение и угол относительно текущего transform
            r.initialLocalPos = Quaternion.Inverse(transform.rotation) * (r.obj.position - transform.position);
            r.initialLocalRot = Quaternion.Inverse(transform.rotation) * r.obj.rotation;
        }
    }

    void LateUpdate()
    {
        Quaternion deltaRot = transform.rotation * Quaternion.Inverse(oldRot);
        Vector3 deltaPos = transform.position - oldPos;

        foreach (var r in related)
        {
            // применяем вращение вокруг pivot
            r.obj.position = transform.position + deltaRot * r.initialLocalPos;
            r.obj.rotation = deltaRot * r.initialLocalRot;
        }

        oldRot = transform.rotation;
        oldPos = transform.position;
    }
}


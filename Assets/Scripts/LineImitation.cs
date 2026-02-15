using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;
using UnityEngine.UIElements;

public class LineImitation : MonoBehaviour
{
    [SerializeField]private SplineContainer splineContainer;
    [SerializeField] private GameObject obj;
    
    

    float tick = 1f;
    float timer = 0f;
    [SerializeField] ConveyorItem[] listItems;
    byte[] line;
    float length;
    float step;
    void Start()
    {
        length = splineContainer.CalculateLength();
        step = 2 / length;
        line = new byte[(int)length];
        for (int i = 0; i < (int)length; i++)
        {

            line[i] = 0;
        }
    }


    void Update()
    {
        if (timer > 0f) { timer -= Time.deltaTime; return; }
        
        for (int i = 0; i < listItems.Length; i++)
        {
            if (line[listItems[i].linePlaceID + 1] == 0)
            {

                listItems[i].position += step;
                MoveItem(listItems[i].gameObject, listItems[i].position);
                line[listItems[i].linePlaceID] = 0;
                line[listItems[i].linePlaceID + 1] = 1;
                listItems[i].linePlaceID += 1;
            }
        }
        timer = tick;
        
    }
    [Serializable]
    class ConveyorItem
    {
        [NonSerialized] public float position;
        public GameObject gameObject;
        [NonSerialized]public bool isMove;
        [NonSerialized] public int linePlaceID = 0;
    }
    private void MoveItem(GameObject item, float linePosition)
    {
        if (splineContainer == null || item == null) return;
        splineContainer.Evaluate(linePosition, out float3 pos, out float3 tan, out float3 up);
        item.transform.position = pos;

        Vector3 forward = ((Vector3)tan).normalized;
        Vector3 upDir = ((Vector3)up).normalized;

        if (forward.sqrMagnitude > 0.0001f)
        {
            item.transform.rotation = Quaternion.LookRotation(forward, upDir);
        }
    }
}

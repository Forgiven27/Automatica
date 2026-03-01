using Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ManipulatorView : MonoBehaviour, IEntity
{
    public uint ID {  get; set; }

    public BoneReference[] bones;
    public Transform basement;

    public void Init(float baseYaw, Dictionary<uint, float> bones)
    {
        UpdateManipulator(baseYaw, bones);
    }


    private void LateUpdate()
    {
        ManipulatorSnapshot snapshot = SimulationAPI.GetManipulator(ID);
        UpdateManipulator(snapshot.baseYaw, snapshot.bones);
    }

    void UpdateManipulator(float baseYaw, Dictionary<uint, float> bonesAngles)
    {
        basement.eulerAngles = new Vector3(basement.eulerAngles.x, baseYaw, basement.eulerAngles.z);
        for (int i = 0; i < bones.Length; i++)
        {
            if (bonesAngles.TryGetValue(bones[i].ID, out float angleX))
            {
                Vector3 eulerAngle = new Vector3(angleX,
                    bones[i].boneRef.transform.eulerAngles.y,
                    bones[i].boneRef.transform.eulerAngles.z);

                //bones[i].boneRef.transform.eulerAngles = eulerAngle;
                bones[i].boneRef.transform.localEulerAngles = eulerAngle;
            }
        }
    }
}
[Serializable]
public class BoneReference
{
    public GameObject boneRef;
    public uint ID;
}
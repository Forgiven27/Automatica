using Simulator;
using System;
using System.Collections.Generic;
using UnityEngine;


public class ManipulatorView : MonoBehaviour, IEntity
{
    public uint ID {  get; set; }

    public BoneReference[] bones;
    public Transform basement;
    public Transform hand;
    [SerializeField] private ItemsInfo _itemsInfo;
    public void Init(float baseYaw, Dictionary<uint, float> bones)
    {
        UpdateManipulator(baseYaw, bones);
    }


    private void LateUpdate()
    {
        ManipulatorSnapshot snapshot = SimulationAPI.GetManipulator(ID);
        UpdateManipulator(snapshot.baseYaw, snapshot.bones);
        if (snapshot.heldItem != null)
        {
            UpdateItem(snapshot.heldItem ?? ItemType.None);
        }
    }



    void UpdateManipulator(float baseYaw, Dictionary<uint, float> bonesAngles)
    {
        basement.localRotation = Quaternion.AngleAxis(baseYaw, Vector3.up);
        for (int i = 0; i < bones.Length; i++)
        {
            if (bonesAngles.TryGetValue(bones[i].ID, out float angleX))
            {
                bones[i].boneRef.transform.localRotation = Quaternion.AngleAxis(angleX, Vector3.right);
            }
        }
    }

    void UpdateItem(ItemType type)
    {

        ItemData item = _itemsInfo.itemsData.Find(x =>  x.type == type);
        Graphics.DrawMesh(item.mesh, hand.localToWorldMatrix, item.material, LayerMask.NameToLayer("Defualt"));
    }

    
}
[Serializable]
public class BoneReference
{
    public GameObject boneRef;
    public uint ID;
}
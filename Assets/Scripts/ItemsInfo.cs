using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "ItemsInfo", menuName = "Scriptable Objects/ItemsInfo")]
public class ItemsInfo : ScriptableObject
{
    public List<ItemData> itemsData;
}
[Serializable]
public class ItemData
{
    public ItemType type;
    public Mesh mesh;
    public Material material;
    public string name;
}

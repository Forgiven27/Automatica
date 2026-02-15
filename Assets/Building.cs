using System;
using UnityEngine;

[Serializable]
public class Building
{
    public GameObject buildingObject;
    public Sprite sprite;
    public BuildingType buildingType;
    public ScriptableObject fullDescription;
    [Space]
    public string header;
    [TextArea(1,10)]
    public string textDescription;
}

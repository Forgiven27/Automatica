using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "BuildBoard", menuName = "Scriptable Objects/BuildBoard")]
public class BuildBoard : ScriptableObject
{
    public BuildButton firstCell;
    public BuildButton secondCell;
    public BuildButton thirdCell;
    public BuildButton forthCell;
    public BuildButton fifthCell;
}
[Serializable]
public class BuildButton
{
    public Building currentBuilding;
    public List<Building> buildings;
}

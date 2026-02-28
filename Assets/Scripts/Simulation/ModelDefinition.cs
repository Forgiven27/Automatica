using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;

[CreateAssetMenu(fileName = "ModelDefinition", menuName = "Scriptable Objects/ModelDefinition")]
public class ModelDefinition : ScriptableObject
{
    public List<MinMaxAABB> minMaxAABBs;
}

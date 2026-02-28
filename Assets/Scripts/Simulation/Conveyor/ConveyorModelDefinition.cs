using System;
using UnityEngine;

namespace Simulator
{
    [CreateAssetMenu(fileName = "ConveyorModel", menuName = "Scriptable Objects/ConveyorModel")]
    public class ConveyorModelDefinition : ScriptableObject
    {
        public float Length;
        public float Width;
        public float Height;

        public BeltDefinition belt;
    }
    [Serializable]
    public class BeltDefinition
    {
        public float Length;
        public float Width;
        public float Height;
        public float Thickness;
    }
}
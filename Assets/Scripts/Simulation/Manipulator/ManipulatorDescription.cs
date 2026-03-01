using UnityEngine;
using Simulator;

[CreateAssetMenu(fileName = "ManipulatorDescription", menuName = "Scriptable Objects/ManipulatorDescription")]
public class ManipulatorDescription : ScriptableObject
{
    public Bone[] bones;
}

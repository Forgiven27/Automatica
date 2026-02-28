using UnityEngine;

[CreateAssetMenu(fileName = "SimulationConfiguration", menuName = "Scriptable Objects/SimulationConfiguration")]
public class SimulationConfiguration : ScriptableObject
{
    public float cellSize;
    public float tickDuration;
}

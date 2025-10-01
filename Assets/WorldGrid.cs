using UnityEngine;

[CreateAssetMenu(fileName = "WorldGrid", menuName = "Scriptable Objects/WorldGrid")]
public class WorldGrid : ScriptableObject
{
    public float cell_step;
    public Vector3 null_position;
}

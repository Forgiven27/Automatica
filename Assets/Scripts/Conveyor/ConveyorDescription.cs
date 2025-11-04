using UnityEngine;

[CreateAssetMenu(fileName = "ConveyorDescription", menuName = "Scriptable Objects/ConveyorDescription")]
public class ConveyorDescription : ScriptableObject
{
    public ConveyorType Type;
    public GameObject prefab;
}

public enum ConveyorType
{
    Straight,
    TurnR,
    TurnL
}

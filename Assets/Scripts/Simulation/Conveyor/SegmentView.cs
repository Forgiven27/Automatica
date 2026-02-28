using UnityEngine;
using Simulator;
public class SegmentView : MonoBehaviour, IEntity
{
    public uint ID {  get; set; }

    public void Bind(uint id)
    {
        ID = id;
    }
}

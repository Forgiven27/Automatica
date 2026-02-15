using System.Collections.Generic;
using UnityEngine;
using Simulator;

[CreateAssetMenu(fileName = "FactoryDescription", menuName = "Scriptable Objects/FactoryDescription")]
public class FactoryDescription : ScriptableObject, IDescription
{
    public FactoryGenerator generator;
    [SerializeField] public List<FactoryPort> ports;
    [SerializeField] public List<FactorySlot> slots;
}

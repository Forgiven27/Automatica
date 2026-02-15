using System.Runtime.Serialization;
using System.Data;
using UnityEngine;
using System;
using System.Text;
using Simulator;

public class FactoryView : MonoBehaviour, IEntity
{
    [SerializeField]private InfoUI infoUI;

    public string ID { get; set; }

    public GameObject factoryMesh;
    public Texture2D icon;
    StringBuilder stringBuilder = new();

    public void Bind(string id)
    {
        ID = id;
    }

    void LateUpdate()
    {
        if (LayerMask.LayerToName(gameObject.layer) == "Building") { 
            var snapshot = SimulationAPI.GetFactory(ID);
            stringBuilder.Clear();
            foreach (var slot in snapshot.slots)
            {
                stringBuilder.AppendLine($"Slot - {slot.ioType} {slot.currentSlotType} {slot.currentCount}/{slot.maxCapacity}");
            }
            foreach (var port in snapshot.ports)
            {
                stringBuilder.AppendLine($"Port - {port.ioType} connection is \"{port.isConnected}\" ItemTypes - {port.ItemTypes}");
            }
            infoUI.UpdateTextUI(stringBuilder.ToString());
        }
    }
}
